using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCMS.DTOs.User;
using OCMS.Entities;
using OCMS.Services.Interfaces;
using OCMS.Shared.Enums;
using OCMS.Shared.Helpers;

namespace OCMS.Controllers
{
    [Authorize]
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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ====== Actions ======
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
           => Json(await _userService.DeleteUserAsync(id));
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
           => Json(await _userService.GetAllUsersAsync());

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
            => Json(await _userService.GetProfileAsync(id));
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] UserFilterDto dto)
                          => Json(await _userService.GetFilteredAsync(dto));

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(Guid id, int status)
                         => Json(await _userService.UpdateStatusAsync(id, status));
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetStats()
                         => Json(await _userService.GetStatsAsync());

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUserByRole(AppRoles role)
        {
            return Ok();
        }
    }
}
