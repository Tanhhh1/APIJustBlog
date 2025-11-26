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
        [ProducesResponseType(typeof(ApiResult<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            var result = await _tokenService.RefreshTokenAsync(refreshTokenDto);
            return Ok(ApiResult<TokenResponse>.Success(result));
        }

        /// <summary>
        /// Revoke Token Async
        /// </summary>
        [AllowAnonymous]
        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            await _tokenService.RevokeTokenAsync(refreshTokenDto);
            return NoContent();
        }
    }
}
