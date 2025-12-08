using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;
using Application.Common.ModelServices;

namespace Application.Interfaces.Services.Auth
{
    public interface IAuthService
    {
        Task<SignUpResponse> SignUpAsync(SignUpDTO dto);
        Task<SignInResponse> SignInAsync(SignInDTO dto);
        Task<TokenResponse> VerifyTwoFactorOtpAsync(VerifyOtpDTO dto);
    }

}
