using OCMS.DTOs.Contact;
using OCMS.Services.Interfaces;
using OCMS.Shared;

namespace OCMS.Services.Implementations
{
    public class ContactService(IEmailService emailService) : IContactService
    {
        private readonly IEmailService _emailService = emailService;

        public async Task<AppResponse> SendMessageAsync(ContactMessageDto dto)
        {
            var body = $@"
                <h3>New Contact Message</h3>
                <p><b>Name:</b> {dto.Name}</p>
                <p><b>Email:</b> {dto.Email}</p>
                <p><b>Phone:</b> {dto.Phone ?? "N/A"}</p>
                <p><b>Subject:</b> {dto.Subject}</p>
                <hr>
                <p>{dto.Message}</p>
            ";

            var sent = await _emailService.SendEmailAsync(
                toEmail: "support@ocms.com",   // apni SmtpSettings.ReceiverEmail yahan use kar sakte hain
                subject: $"[Contact] {dto.Subject} — {dto.Name}",
                htmlBody: body,
                replyToEmail: dto.Email,
                replyToName: dto.Name
            );

            return sent
                ? AppResponse.Ok("Your message has been sent successfully.")
                : AppResponse.Fail("Failed to send message. Please try again later.");
        }
    }
}