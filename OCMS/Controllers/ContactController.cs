using Microsoft.AspNetCore.Mvc;
using OCMS.DTOs.Contact;
using OCMS.Services.Interfaces;

namespace OCMS.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] ContactMessageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Subject) ||
                string.IsNullOrWhiteSpace(dto.Message))
            {
                return Json(OCMS.Shared.AppResponse.Fail("Please fill all required fields."));
            }

            return Json(await _contactService.SendMessageAsync(dto));
        }
    }
}