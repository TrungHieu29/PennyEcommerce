using PennyEcommerce.Models;
namespace PennyEcommerce.Services;

public interface IProductService
{
    List<Product> GetAllProducts();
    Product? GetProductById(int id);

    void AddProduct(Product product);

    void UpdateProduct(Product product);

    void DeleteProduct(int id);

    List<ProductSummaryDto> GetProductSummaries();
}

public record ProductSummaryDto(
    int Id,
    string Name,
    decimal? Price
);