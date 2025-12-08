using Application.Exceptions;
using Application.Interfaces.Services.Auth;
using Application.Interfaces.UnitOfWork;
using Application.Models;
using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;
using Domain.Entities;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Helpers;
using Shared.Logger;
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

        public AuthService(
            UserManager<AppUser> userManager,
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

        // Sign-in
        public async Task<SignInResponse> SignInAsync(SignInDTO request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username)
                           ?? await _userManager.FindByEmailAsync(request.Username);

                if (user == null)
                {
                    Logging.Warning("Sign-in failed: Username or Email '{Username}' not found", request.Username);
                    throw new UnauthorizedException("Incorrect username or password.");
                }

                var checkPw = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!checkPw.Succeeded)
                {
                    Logging.Warning("Sign-in failed: Incorrect password for Username '{Username}'", request.Username);
                    throw new UnauthorizedException("Incorrect username or password.");
                }

                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                await _emailService.SendEmailAsync(new EmailMessage
                {
                    To = user.Email!,
                    Subject = "Your OTP Login Code",
                    Content = $"Your OTP code is: <b>{otp}</b>"
                });

                await _signInManager.SignInAsync(user, false);

                Logging.Info("Sign-in successful: OTP sent to user '{Username}'", request.Username);

                return new SignInResponse
                {
                    TwoFactorRequired = true,
                    Message = "OTP has been sent to your email."
                };
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Unhandled exception during SignIn for Username {Username}", request.Username);
                throw;
            }
        }

        // Sign-up
        public async Task<SignUpResponse> SignUpAsync(SignUpDTO request)
        {
            var email = request.Email.NormalizeString("");

            var userCheckEmail = await _userManager.Users
                .FirstOrDefaultAsync(user => user.Email == email);
            if (userCheckEmail != null)
            {
                Logging.Warning("Sign-up failed: Email '{Email}' already exists.", email);
                throw new BadRequestException("Email already exists.");
            }

            var checkUserName = await _userManager.Users
                .FirstOrDefaultAsync(x => x.NormalizedUserName == request.UserName!.ToUpper());
            if (checkUserName != null)
            {
                Logging.Warning("Sign-up failed: Username '{Username}' already exists.", request.UserName);
                throw new BadRequestException("Username already exists.");
            }

            if (request.Password != request.ConfirmPassword)
            {
                Logging.Warning("Sign-up failed: Password and ConfirmPassword do not match for Username '{Username}'", request.UserName);
                throw new BadRequestException("Passwords do not match.");
            }

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
                await _userManager.AddToRoleAsync(userNew, UserRoleConst.Member);
                Logging.Info("User registered successfully: Username '{Username}', Email '{Email}'",
                    request.UserName, email);
                return new SignUpResponse() { Ok = true };
            }
            Logging.Error("Error registering account: {Errors}", result.Errors);
            throw new BadRequestException("Account registration failed.");
        }

        // OTP Verification
        public async Task<TokenResponse> VerifyTwoFactorOtpAsync(VerifyOtpDTO request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                {
                    Logging.Warning("OTP verification failed: User '{Username}' not found.", request.Username);
                    throw new BadRequestException("User not found.");
                }

                var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", request.Otp);

                if (!isValid)
                {
                    Logging.Warning("OTP verification failed: Invalid OTP for User '{Username}'", request.Username);
                    throw new BadRequestException("Invalid OTP.");
                }

                Logging.Info("OTP verification succeeded for User '{Username}'", request.Username);
                return await HandleGenerateToken(user);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error during OTP verification for Username {Username}", request.Username);
                throw;
            }
        }

        // Generate JWT Token
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

                Logging.Info("JWT token generated for User '{Username}'", user.UserName);

                return new TokenResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.RefreshToken,
                    Expires = expireStart.AddMinutes(_jwtSetting.TokenValidityInMinutes),
                };
            }
            catch (Exception e)
            {
                Logging.Error(e, "Error generating token for User '{Username}'", user.UserName);
                throw new BadRequestException(e.Message);
            }
        }
    }
}
