using Microsoft.AspNetCore.Mvc;
using PennyEcommerce.Services.Auth;
using PennyEcommerce.ViewModels.Auth;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        bool result = await _authService.RegisterAsync(model.Email, model.Password);
        if (!result)
        {
            ModelState.AddModelError(string.Empty, "Email đã tồn tại");
            return View(model);
        }
        return RedirectToAction("Index", "Home");
    }
}