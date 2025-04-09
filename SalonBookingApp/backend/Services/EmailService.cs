using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace SalonBookingApp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Create a new MimeMessage
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // Set email body (HTML can be used)
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            // Set up and connect to the SMTP server using MailKit
            using var smtp = new SmtpClient();
            // The SMTP settings can be stored in appsettings.json under EmailSettings
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "2525");
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];

            await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
