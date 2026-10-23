using PennyEcommerce.Models.Auth;

namespace PennyEcommerce.Services.Auth;

public interface IAuthService
{
    Task<bool> RegisterAsync(string email, string password);
    Task<AppUser?> LoginAsync(string email, string password);
    Task<List<string>> GetUserRolesAsync(int userId);
}