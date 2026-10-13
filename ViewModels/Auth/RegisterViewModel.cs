using System.ComponentModel.DataAnnotations;
namespace PennyEcommerce.ViewModels.Auth;

public class RegisterViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
    ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;
}