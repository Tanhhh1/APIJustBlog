using Application.Models;


namespace Application.Interfaces.Auth
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
