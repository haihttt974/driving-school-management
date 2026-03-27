using System.Net;
using System.Net.Mail;
using driving_school_management.Helpers;
using Microsoft.Extensions.Options;

namespace driving_school_management.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendNotificationEmailAsync(string toEmail, string subject, string title, string content);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly EmailSettings _emailSettings;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            IEmailTemplateService emailTemplateService)
        {
            _emailSettings = emailSettings.Value;
            _emailTemplateService = emailTemplateService;
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderPassword
                ),
                EnableSsl = true
            };
        }

        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            return message;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var template = await _emailTemplateService.GetTemplateAsync("OtpEmailTemplate.html");

            var body = _emailTemplateService.ReplacePlaceholders(template, new Dictionary<string, string>
            {
                { "{{OTP_CODE}}", otpCode },
                { "{{EXPIRE_SECONDS}}", "60 giây" }
            });

            using var message = CreateMailMessage(
                toEmail,
                "Mã OTP xác nhận đăng ký - GPLX System",
                body
            );

            using var client = CreateSmtpClient();
            await client.SendMailAsync(message);
        }

        public async Task SendNotificationEmailAsync(string toEmail, string subject, string title, string content)
        {
            var template = await _emailTemplateService.GetTemplateAsync("NotificationEmailTemplate.html");

            var body = _emailTemplateService.ReplacePlaceholders(template, new Dictionary<string, string>
            {
                { "{{EMAIL_SUBJECT}}", subject },
                { "{{TITLE}}", title },
                { "{{CONTENT}}", content }
            });

            using var message = CreateMailMessage(toEmail, subject, body);

            using var client = CreateSmtpClient();
            await client.SendMailAsync(message);
        }
    }
}