using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OCMS.DTOs.Auth;
using OCMS.Services.Interfaces;

namespace OCMS.Controllers
{
    public class AuthController(IAuthService authService) : Controller
    {
        private readonly IAuthService _authService = authService;

        [HttpGet] public IActionResult LoginPage() => View();
        [HttpGet] public IActionResult RegisterPage() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
            => Json(await _authService.RegisterAsync(dto));

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
                                            => Json(await _authService.LoginAsync(dto));
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("LoginPage");
        }
    }
}
