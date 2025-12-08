using Application.Models;


namespace Application.Interfaces.Services.Auth
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
