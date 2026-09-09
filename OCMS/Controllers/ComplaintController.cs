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
        [HttpGet] public IActionResult Submit() => View();
        [HttpGet] public IActionResult Track() => View();

        // ====== Actions ======
        [HttpPost]
        public async Task<IActionResult> Submit([FromForm] AddComplaintDto dto)
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _complaintService.SubmitAsync(dto, userId));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Json(await _complaintService.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = ClaimsHelper.GetUserId(User);
            return Json(await _complaintService.GetByUserIdAsync(userId));
        }

        [HttpGet]
        public async Task<IActionResult> TrackById(string trackId)
            => Json(await _complaintService.GetByTrackIdAsync(trackId));
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
                                         => Json(await _complaintService.GetByIdAsync(id));
        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] ComplaintFilterDto dto)
                                         => Json(await _complaintService.GetFilteredAsync(dto));
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(Guid id, int status)
                                         => Json(await _complaintService.UpdateStatusAsync(id, status));

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
                                         => Json(await _complaintService.DeleteAsync(id));
    }
}
