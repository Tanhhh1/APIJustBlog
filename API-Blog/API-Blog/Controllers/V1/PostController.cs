using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_Blog.Controllers.V1
{
    public class PostController : ApiController
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPost()
        {
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
        public async Task<IActionResult> CreatePost([FromBody] PostSaveDTO createDTO)
        {
            var create = await _postService.CreatePostAsync(createDTO);
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string keyword)
        {
            var result = await _postService.SearchAsync(keyword);
            if (!result.Any())
                return NotFound(ApiResult<PostResponse>.Failure(new List<string> { "No matching posts found" }));

            return Ok(result);
        }
    }
}
