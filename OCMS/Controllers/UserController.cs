using Microsoft.AspNetCore.Mvc;
using OCMS.DTOs.User;
using OCMS.Services.Interfaces;
using OCMS.Shared.Helpers;

namespace OCMS.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ====== Page ======
        [HttpGet]
        public IActionResult Profile() => View();
        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ====== Actions ======
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _userService.GetProfileAsync(userId));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _userService.UpdateProfileAsync(userId, dto));
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _userService.ChangePasswordAsync(userId, dto));
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile imageFile)
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _userService.UploadImageAsync(userId, imageFile));
        }
    }
}
