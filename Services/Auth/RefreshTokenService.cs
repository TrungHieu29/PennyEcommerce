using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PennyEcommerce.Models.Auth;

namespace PennyEcommerce.Services.Auth;

public class RefreshTokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(7);
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly JwtTokenService _jwtTokenService;

    public RefreshTokenService(
        AppDbContext context,
        IAuthService authService,
        JwtTokenService jwtTokenService)
    {
        _context = context;
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<RefreshTokenIssueResult> CreateAsync(int userId)
    {
        var token = GenerateToken();
        var now = DateTime.UtcNow;
        _context.RefreshTokens.Add(new AppRefreshToken
        {
            UserId = userId,
            TokenHash = HashToken(token),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.Add(TokenLifetime)
        });

        await _context.SaveChangesAsync();
        return new RefreshTokenIssueResult(token, now.Add(TokenLifetime));
    }

    public async Task<RefreshTokenRotationResult?> RotateAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _context.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash);
        var now = DateTime.UtcNow;

        if (storedToken == null ||
            storedToken.RevokedAtUtc != null ||
            storedToken.ExpiresAtUtc <= now ||
            !storedToken.User.IsActive)
        {
            await transaction.RollbackAsync();
            return null;
        }

        int updated = await _context.RefreshTokens
            .Where(token => token.Id == storedToken.Id &&
                            token.RevokedAtUtc == null &&
                            token.ExpiresAtUtc > now)
            .ExecuteUpdateAsync(update => update
                .SetProperty(token => token.RevokedAtUtc, now));

        if (updated != 1)
        {
            await transaction.RollbackAsync();
            return null;
        }

        var newRefreshToken = GenerateToken();
        var newRefreshTokenExpiresAtUtc = now.Add(TokenLifetime);
        _context.RefreshTokens.Add(new AppRefreshToken
        {
            UserId = storedToken.UserId,
            TokenHash = HashToken(newRefreshToken),
            CreatedAtUtc = now,
            ExpiresAtUtc = newRefreshTokenExpiresAtUtc
        });

        var roles = await _authService.GetUserRolesAsync(storedToken.UserId);
        var accessToken = _jwtTokenService.CreateToken(storedToken.User, roles);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new RefreshTokenRotationResult(
            accessToken,
            newRefreshToken,
            newRefreshTokenExpiresAtUtc);
    }

    public async Task RevokeAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var tokenHash = HashToken(refreshToken);
        var now = DateTime.UtcNow;
        await _context.RefreshTokens
            .Where(token => token.TokenHash == tokenHash && token.RevokedAtUtc == null)
            .ExecuteUpdateAsync(update => update
                .SetProperty(token => token.RevokedAtUtc, now));
    }

    private static string GenerateToken()
    {
        return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
            RandomNumberGenerator.GetBytes(32));
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}

public record RefreshTokenRotationResult(
    JwtTokenResult AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);

public record RefreshTokenIssueResult(
    string Token,
    DateTime ExpiresAtUtc);
