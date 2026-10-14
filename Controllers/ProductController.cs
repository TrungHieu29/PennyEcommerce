using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        _productService.DeleteProduct(id);

        return RedirectToAction(nameof(Index));
    }
}