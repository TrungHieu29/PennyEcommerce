using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyEcommerce.Models;
using PennyEcommerce.Services;

namespace PennyEcommerce.Controllers.Api;

[ApiController]
[Route("api/products")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsApiController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_productService.GetAllProducts());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var product = _productService.GetProductById(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public IActionResult Create([FromBody] Product product)
    {
        _productService.AddProduct(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
