using System.ComponentModel.DataAnnotations;

namespace PennyEcommerce.Models.Auth;

public class AppRefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(64)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public AppUser User { get; set; } = null!;
}
