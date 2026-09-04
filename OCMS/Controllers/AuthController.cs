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
        [HttpGet] public IActionResult VerifyEmailPage() => View();
        [HttpGet] public IActionResult ResetPasswordPage() => View();

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
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> VerifyEmail([FromBody] ForgotPasswordDto dto)
        {
            var response = await _authService.ForgotPasswordAsync(dto);

            if (response.Success && response.Data is Guid userId)
                TempData["userId"] = userId;

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
            => Json(await _authService.ResetPasswordAsync(dto));
    }
}