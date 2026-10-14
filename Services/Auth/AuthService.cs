using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PennyEcommerce.Models.Auth;

namespace PennyEcommerce.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;

    public AuthService(
        AppDbContext context,
        PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<bool> RegisterAsync(
        string email,
        string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        string normalizedEmail = email.Trim().ToUpperInvariant();

        bool exists = await _context.Users
            .AnyAsync(u => u.NormalizedEmail == normalizedEmail);

        if (exists)
        {
            return false;
        }

        var result = _passwordService.HashPassword(password);

        var user = new AppUser
        {
            Email = email.Trim(),
            NormalizedEmail = normalizedEmail,
            PasswordHash = result.Hash,
            PasswordSalt = result.Salt,
            PasswordIterations = result.Iterations
        };

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (
            ex.GetBaseException() is SqlException sqlEx &&
            (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            return false;
        }
    }

    public async Task<AppUser?> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }
        string normalizedEmail = email.Trim().ToUpperInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (user == null || !user.IsActive)
        {
            return null;
        }

        bool isValid = _passwordService.VerifyPassword(
            password,
            user.PasswordHash,
            user.PasswordSalt,
            user.PasswordIterations
        );

        if (!isValid)
        {
            return null;
        }

        return user;
    }
}