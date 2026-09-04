using Microsoft.Extensions.Options;
using OCMS.Services.Interfaces;
using OCMS.Shared;
using System.Net;
using System.Net.Mail;

namespace OCMS.Services.Implementations
{
    public class EmailService(IOptions<SmtpSettings> smtpOptions) : IEmailService
    {
        private readonly SmtpSettings _smtp = smtpOptions.Value;

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? replyToEmail = null, string? replyToName = null)
        {
            try
            {
                var mail = new MailMessage
                {
                    From = new MailAddress(_smtp.SenderEmail, _smtp.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                if (!string.IsNullOrWhiteSpace(replyToEmail))
                    mail.ReplyToList.Add(new MailAddress(replyToEmail, replyToName ?? replyToEmail));

                using var client = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    Credentials = new NetworkCredential(_smtp.SenderEmail, _smtp.SenderPassword),
                    EnableSsl = true
                };

                await client.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}