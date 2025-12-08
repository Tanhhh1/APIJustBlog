using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces.Services.Auth;
using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Blog.Controllers.V1.Auth
{
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Register Async
        /// </summary>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(ApiResult<SignInResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO request)
        {
            var result = await _authService.SignUpAsync(request);
            return Ok(ApiResult<SignUpResponse>.Success(result));
        }
        /// <summary>
        /// Sign In 
        /// </summary>
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(ApiResult<SignUpResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO request)
        {
            var result = await _authService.SignInAsync(request);
            return Ok(ApiResult<SignInResponse>.Success(result));
        }

        /// <summary>
        /// Verify OTP
        /// </summary>
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(ApiResult<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyTwoFactorOtp([FromBody] VerifyOtpDTO request)
        {
            var result = await _authService.VerifyTwoFactorOtpAsync(request);
            return Ok(ApiResult<TokenResponse>.Success(result));
        }
    }
}
