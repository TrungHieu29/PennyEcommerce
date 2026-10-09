using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
namespace PennyEcommerce.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(120, ErrorMessage = "Tên sản phẩm không được vượt quá 120 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giá không được để trống")]
    [Range(0.01, 1000000000,
        ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Số lượng không được để trống")]
    [Range(0, 100000,
        ErrorMessage = "Số lượng không hợp lệ")]
    public int? StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;
}