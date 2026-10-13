namespace PennyEcommerce.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(string email, string password);
}