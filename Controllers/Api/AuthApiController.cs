using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PennyEcommerce.Services.Auth;

namespace PennyEcommerce.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtTokenService _jwtTokenService;
    private readonly RefreshTokenService _refreshTokenService;
    private const string RefreshCookieName = "penny_refresh_token";

    public AuthApiController(
        IAuthService authService,
        JwtTokenService jwtTokenService,
        RefreshTokenService refreshTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ApiLoginRequest request)
    {
        try
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _authService.GetUserRolesAsync(user.Id);
            var token = _jwtTokenService.CreateToken(user, roles);
            var refreshToken = await _refreshTokenService.CreateAsync(user.Id);
            SetRefreshCookie(refreshToken.Token, refreshToken.ExpiresAtUtc);

            return Ok(new
            {
                accessToken = token.AccessToken,
                tokenType = "Bearer",
                expiresAtUtc = token.ExpiresAtUtc
            });
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!IsSameOriginRequest())
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken);
        try
        {
            var result = await _refreshTokenService.RotateAsync(refreshToken);
            if (result == null)
            {
                Response.Cookies.Delete(RefreshCookieName, GetRefreshCookieOptions());
                return Unauthorized();
            }

            SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);
            return Ok(new
            {
                accessToken = result.AccessToken.AccessToken,
                tokenType = "Bearer",
                expiresAtUtc = result.AccessToken.ExpiresAtUtc
            });
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!IsSameOriginRequest())
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        Request.Cookies.TryGetValue(RefreshCookieName, out var refreshToken);
        try
        {
            await _refreshTokenService.RevokeAsync(refreshToken);
            Response.Cookies.Delete(RefreshCookieName, GetRefreshCookieOptions());
            return NoContent();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private void SetRefreshCookie(string token, DateTime expiresAtUtc)
    {
        var options = GetRefreshCookieOptions();
        options.Expires = new DateTimeOffset(DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc));
        Response.Cookies.Append(RefreshCookieName, token, options);
    }

    private static CookieOptions GetRefreshCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth"
        };
    }

    private bool IsSameOriginRequest()
    {
        var source = Request.Headers.Origin.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(source))
        {
            source = Request.Headers.Referer.FirstOrDefault();
        }

        return Uri.TryCreate(source, UriKind.Absolute, out var sourceUri) &&
               string.Equals(sourceUri.Scheme, Request.Scheme, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(sourceUri.Authority, Request.Host.Value, StringComparison.OrdinalIgnoreCase);
    }
}

public class ApiLoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
