using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCMS.DTOs.Complaint;
using OCMS.Services.Interfaces;
using OCMS.Shared.Helpers;

namespace OCMS.Controllers
{
    [Authorize]
    public class ComplaintController : Controller
    {
        private readonly IComplaintService _complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        // ====== Pages (GET) ======
        [HttpGet] public IActionResult Index() => View();
        [AllowAnonymous]
        [HttpGet] public IActionResult Submit() => View();
        [AllowAnonymous]
        [HttpGet] public IActionResult Track() => View();

        // ====== Actions ======
        [HttpPost]
        public async Task<IActionResult> Submit([FromForm] AddComplaintDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _complaintService.SubmitAsync(dto, userId));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> TrackById(string trackId)
            => Json(await _complaintService.GetByTrackIdAsync(trackId));

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Json(await _complaintService.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _complaintService.GetByUserIdAsync(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
                                         => Json(await _complaintService.GetByIdAsync(id));
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] ComplaintFilterDto dto)
                                         => Json(await _complaintService.GetFilteredAsync(dto));
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(Guid id, int status)
                                         => Json(await _complaintService.UpdateStatusAsync(id, status));

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
                                         => Json(await _complaintService.DeleteAsync(id));
    }
}
