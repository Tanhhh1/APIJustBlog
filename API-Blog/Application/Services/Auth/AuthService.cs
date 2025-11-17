using Application.Exceptions;
using Application.Interfaces.Auth;
using Application.Models;
using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;
using Application.UnitOfWork;
using Domain.Entities;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Helpers;
using Shared.Models;

namespace Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtSetting _jwtSetting;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IOptionsMonitor<JwtSetting> jwtSetting, 
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSetting = jwtSetting.CurrentValue;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<SignInResponse> SignInAsync(SignInDTO request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                    user = await _userManager.FindByEmailAsync(request.Username);

                if (user == null)
                    throw new Exception("Incorrect username or password.");

                var checkPw = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!checkPw.Succeeded)
                    throw new Exception("Incorrect username or password.");
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                await _emailService.SendEmailAsync(new EmailMessage
                {
                    To = user.Email!,
                    Subject = "Your OTP Login Code",
                    Content = $"Your OTP code is: <b>{otp}</b>"
                });

                await _signInManager.SignInAsync(user, false);

                return new SignInResponse
                {
                    TwoFactorRequired = true,
                    Message = "OTP has been sent to your email."
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SignUpResponse> SignUpAsync(SignUpDTO request)
        {
            var email = request.Email.NormalizeString("");

            var userCheckEmail = await _userManager.Users
                .FirstOrDefaultAsync(user => user.Email == email);
            if (userCheckEmail != null)
                throw new BadRequestException("Email already exists.");

            var checkUserName = await _userManager.Users
                .FirstOrDefaultAsync(x => x.NormalizedUserName == request.UserName!.ToUpper());
            if (checkUserName != null)
                throw new BadRequestException("Username already exists.");

            if (request.Password != request.ConfirmPassword)
                throw new BadRequestException("Passwords do not match.");

            var userNew = new AppUser()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName.Trim(),
                NormalizedUserName = request.UserName.ToUpper().Trim(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                DateBirth = DateTime.UtcNow,
                TwoFactorEnabled = true,
                CreatedOn = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(userNew, request.Password!.NormalizeString(""));
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userNew, UserRoleConst.Admin);
                return new SignUpResponse()
                {
                    Ok = true
                };
            }
            throw new BadRequestException("Account registration failed.");
        }


        public async Task<TokenResponse> VerifyTwoFactorOtpAsync(string username, string otp)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new Exception("User not found.");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", otp);

            if (!isValid)
                throw new Exception("Invalid OTP.");

            return await HandleGenerateToken(user);
        }



        private async Task<TokenResponse> HandleGenerateToken(AppUser user)
        {
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
                var expireStart = DateTime.UtcNow;
                var accessToken = JwtHelper.GenerateToken(roles, _jwtSetting, userInformation, expireStart);
                var refreshToken = new UserRefreshToken()
                {
                    AccessToken = accessToken!,
                    ExpiryTime = expireStart.AddMinutes(_jwtSetting.TokenValidityInMinutes),
                    RefreshToken = $"{Guid.NewGuid():N}_{StringHelper.RandomString()}",
                    UserId = user.Id.ToString(),
                    IsRevoked = false,
                    IsUsed = false,
                };
                await _unitOfWork.UserRefreshTokenRepository.AddAsync(refreshToken);
                await _unitOfWork.CompleteAsync();
                return new TokenResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RefreshToken,
                    Expires = expireStart.AddMinutes(_jwtSetting.TokenValidityInMinutes),
                };
            }
            catch (Exception e)
            {
                throw new BadRequestException(e.Message);
            }
        }
    }
}
