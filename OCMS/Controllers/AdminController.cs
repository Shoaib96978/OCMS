using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCMS.Services.Interfaces;
using OCMS.Shared.Helpers;

namespace OCMS.Controllers
{
    //[Authorize(Roles = AppRoles.Admin)]
    public class AdminController(IAdminService adminService) : Controller
    {
        private readonly IAdminService _adminService = adminService;

        [HttpGet]
        public IActionResult Index() => View();
        [HttpGet]
        public async Task<IActionResult> Users() => View();
        [HttpGet]
        public async Task<IActionResult> Complaints() => View();
        [HttpGet]
        public async Task<IActionResult> GetStats()
            => Json(await _adminService.GetDashboardStatsAsync());
           
    }
}