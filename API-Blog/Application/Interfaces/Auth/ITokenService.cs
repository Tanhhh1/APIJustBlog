using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;

namespace Application.Interfaces.Auth;

public interface ITokenService
{
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto);
    Task<bool> RevokeTokenAsync(RefreshTokenRequestDto refreshTokenDto);
}