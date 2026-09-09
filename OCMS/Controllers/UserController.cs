using Microsoft.AspNetCore.Mvc;
using OCMS.DTOs.User;
using OCMS.Entities;
using OCMS.Services.Interfaces;
using OCMS.Shared.Enums;
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
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
           => Json(await _userService.DeleteUserAsync(id));
        [HttpGet]
        public async Task<IActionResult> GetAll()
           => Json(await _userService.GetAllUsersAsync());

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
            => Json(await _userService.GetProfileAsync(id));
        [HttpPut]
        public async Task<IActionResult> ToggleStatus(Guid id)
            => Json(await _userService.ToggleStatusAsync(id));
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

        [HttpGet]
        public async Task<IActionResult> GetUserByRole(AppRoles role)
        {
            return Ok();
        }
    }
}
