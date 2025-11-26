using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ClaimService;
using Shared.Logger;

namespace API_Blog.Controllers.V1
{
    public class PostController : ApiController
    {
        private readonly IPostService _postService;
        private readonly ClaimService _claimService;
        public PostController(IPostService postService, ClaimService claimService)
        {
            _postService = postService;
            _claimService = claimService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPost([FromQuery] string keyword = null)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var result = await _postService.SearchAsync(keyword);
                if (!result.Any())
                    return NotFound(ApiResult<PostResponse>.Failure(new List<string> { "No matching posts found" }));

                return Ok(result);
            }
            return Ok(ApiResult<IEnumerable<PostDTO>>.Success(await _postService.GetAllPostAsync()));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByPostId(int id)
        {
            var post = await _postService.GetByPostIdAsync(id);
            if (post == null)
                return NotFound(ApiResult<PostDTO>.Failure(new List<string> { "Post not found " }));
            return Ok(ApiResult<PostDTO>.Success(post));
        }
        [HttpPost]  
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreatePost([FromBody] PostSaveDTO createDTO)
        {
            var userId = _claimService.UserId;
            if (userId == Guid.Empty)
                return Unauthorized();
            var create = await _postService.CreatePostAsync(createDTO, userId);
            return Ok(ApiResult<PostResponse>.Success(create));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost([FromBody] PostSaveDTO updateDTO, int id)
        {
            var update = await _postService.UpdatePostAsync(id, updateDTO);
            if(!update.Ok)
                return NotFound(ApiResult<PostResponse>.Failure(new List<string> { "Post not found "}));
            return Ok(ApiResult<PostResponse>.Success(update));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var delete = await _postService.DeletePostAsync(id);
            if(!delete.Ok)
                return NotFound(ApiResult<PostResponse>.Failure(new List<string> { "Post not found" }));
            return Ok(ApiResult<PostResponse>.Success(delete));
        }
    }
}
