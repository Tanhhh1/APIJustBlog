using Application.Exceptions;
using Application.Interfaces.Services.Auth;
using Application.Interfaces.UnitOfWork;
using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;
using Domain.Entities;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Helpers;
using Shared.Logger;
using Shared.Models;
using System.Security.Authentication;

namespace Application.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSetting _jwtSetting;

        public TokenService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IOptionsMonitor<JwtSetting> jwtSetting)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtSetting = jwtSetting.CurrentValue;
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto)
        {
            var userId = JwtHelper.VerifyToken(_jwtSetting, refreshTokenDto?.AccessToken);
            if (userId == null || refreshTokenDto == null) throw new UnauthorizedException("Unauthorized");

            var user = await _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            var oldRefreshToken = await _unitOfWork.UserRefreshTokenRepository.GetByCondition(x => x.UserId == userId
                    && x.AccessToken == refreshTokenDto.AccessToken
                    && x.RefreshToken == refreshTokenDto.RefreshToken)
                .FirstOrDefaultAsync();

            if (user == null || oldRefreshToken == null || oldRefreshToken.IsUsed || oldRefreshToken.IsRevoked)
                throw new UnauthorizedException("Unauthorized");

            var refreshTokenValidityInDays = _jwtSetting.RefreshTokenValidityInDays;
            var dateTimeNow = DateTime.UtcNow;
            var expiryTime = oldRefreshToken.ExpiryTime;
            var difference = dateTimeNow - expiryTime;

            if (expiryTime < dateTimeNow)
                throw new UnauthorizedException("Unauthorized");

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInformation = new JwtUserInformation()
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.FirstName,
                    UniqueName = user.UserName
                };

                var accessToken = JwtHelper.GenerateToken(roles, _jwtSetting, userInformation, dateTimeNow);
                oldRefreshToken.IsUsed = true;
                await _unitOfWork.UserRefreshTokenRepository.UpdateAsync(oldRefreshToken);

                var refreshToken = new UserRefreshToken()
                {
                    AccessToken = accessToken!,
                    ExpiryTime = dateTimeNow.AddMinutes(_jwtSetting.TokenValidityInMinutes),
                    RefreshToken = $"{Guid.NewGuid():N}_{StringHelper.RandomString()}",
                    UserId = user.Id.ToString(),
                    IsRevoked = false,
                    IsUsed = false,
                };

                await _unitOfWork.UserRefreshTokenRepository.AddAsync(refreshToken);
                await _userManager.UpdateAsync(user);
                await _unitOfWork.CompleteAsync();
                return new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RefreshToken,
                    Expires = dateTimeNow.AddMinutes(_jwtSetting.TokenValidityInMinutes),
                };
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error during RevokeToken for UserId {UserId}", userId ?? "unknown");
                throw new BadRequestException("Error get refresh token");
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
        }

        public async Task<bool> RevokeTokenAsync(RefreshTokenRequestDto refreshTokenDto)
        {
            var userId = JwtHelper.VerifyToken(_jwtSetting, refreshTokenDto.AccessToken);
            if (userId == null || refreshTokenDto == null) throw new AuthenticationException("Unauthorized");

            var oldRefreshToken = await _unitOfWork.UserRefreshTokenRepository.GetByCondition(x => x.UserId == userId
                    && x.AccessToken == refreshTokenDto.AccessToken
                    && x.RefreshToken == refreshTokenDto.RefreshToken)
                .FirstOrDefaultAsync();
            try
            {
                if (oldRefreshToken != null)
                {
                    oldRefreshToken.IsRevoked = true;
                    await _unitOfWork.UserRefreshTokenRepository.UpdateAsync(oldRefreshToken);
                    await _unitOfWork.CompleteAsync();
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
            }
            finally
            {
                await _unitOfWork.DisposeAsync();
            }
            return true;
        }
    }
}
