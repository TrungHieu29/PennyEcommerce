using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PennyEcommerce.Models.Auth;

namespace PennyEcommerce.Services.Auth;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public JwtTokenResult CreateToken(AppUser user, IEnumerable<string> roles)
    {
        var key = new SymmetricSecurityKey(
            Convert.FromBase64String(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(
            _configuration.GetValue<int>("Jwt:ExpirationMinutes", 30));
        var claims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new("email", user.Email)
        };
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var payload = new JwtPayload(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            now,
            expiresAtUtc,
            now);
        var token = new JwtSecurityToken(
            new JwtHeader(credentials),
            payload);

        return new JwtTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }
}

public record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);
