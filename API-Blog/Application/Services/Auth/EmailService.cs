using Application.Interfaces.Services.Auth;
using Application.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shared.Logger;

namespace Application.Services.Auth
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _settings;

        public EmailService(IOptions<EmailSetting> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.From));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;

            email.Body = new TextPart("html")
            {
                Text = message.Content
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                Logging.Info("Email sent successfully to {To} with subject '{Subject}'", message.To, message.Subject);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Failed to send email to {To}", message.To);
                throw;
            }
        }
    }

}
