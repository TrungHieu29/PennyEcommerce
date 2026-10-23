using PennyEcommerce.Models;
using PennyEcommerce.Services;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products.AsNoTracking().ToList();
    }

    public Product? GetProductById(int id)
    {
        return _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);
    }

    public void AddProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void UpdateProduct(Product product)
    {
        var existing = _context.Products
            .FirstOrDefault(p => p.Id == product.Id);

        if (existing == null)
            return;

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.StockQuantity = product.StockQuantity;
        existing.IsActive = product.IsActive;
        _context.Entry(existing)
            .Property(p => p.RowVersion)
            .OriginalValue = product.RowVersion;
        _context.SaveChanges();
    }

    public void DeleteProduct(int id)
    {
        var product = _context.Products
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
            return;

        product.IsDeleted = true;

        _context.SaveChanges();
    }

    public List<ProductSummaryDto> GetProductSummaries()
    {
        return _context.Products
            .AsNoTracking()
            .Select(p => new ProductSummaryDto(
                p.Id,
                p.Name,
                p.Price
            ))
            .ToList();
    }
}
