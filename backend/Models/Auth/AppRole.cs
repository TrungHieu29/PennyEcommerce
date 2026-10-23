using System.ComponentModel.DataAnnotations;

namespace PennyEcommerce.Models.Auth;

public class AppRole
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NormalizedName { get; set; } = string.Empty;
}