using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces.Auth;
using Application.Models.Auth.DTO;
using Application.Models.Auth.Response;
using Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Blog.Controllers.V1.Auth
{
    public class TokenController : ApiController
    {
        private readonly ITokenService _tokenService;
        public TokenController (ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Refresh Token Async
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            try
            {
                var result = await _tokenService.RefreshTokenAsync(refreshTokenDto);

                return Ok(ApiResult<TokenResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<SignUpResponse>.Failure(new[]
                {
                    ex.Message
                }));
            }
        }

        /// <summary>
        /// Revoke Token Async
        /// </summary>
        [AllowAnonymous]
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            await _tokenService.RevokeTokenAsync(refreshTokenDto);
            return NoContent();
        }
    }
}
