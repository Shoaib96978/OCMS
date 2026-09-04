namespace OCMS.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? replyToEmail = null, string? replyToName = null);
    }
}