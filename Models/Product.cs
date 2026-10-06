using System.ComponentModel.DataAnnotations;
namespace PennyEcommerce.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1000000000)]
    public decimal Price { get; set; }

    [Range(0, 100000)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;
}