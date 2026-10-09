using Microsoft.AspNetCore.Mvc;

using PennyEcommerce.Models;
using PennyEcommerce.Services;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        var products = _productService.GetAllProducts();
        return View(products);
    }

    public IActionResult Details(int id)
    {
        var product = _productService.GetProductById(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _productService.AddProduct(product);

            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var product = _productService.GetProductById(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _productService.UpdateProduct(product);
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var product = _productService.GetProductById(id);

        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _productService.DeleteProduct(id);

        return RedirectToAction(nameof(Index));
    }
}