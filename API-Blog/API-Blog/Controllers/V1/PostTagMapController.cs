using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.DTOs.PostTagMap;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Blog.Controllers.V1
{
    [EnableRateLimiting("CrudPolicy")]
    public class PostTagMapController : ApiController
    {
        private readonly IPostTagMapService _service;

        public PostTagMapController(IPostTagMapService service)
        {
            _service = service;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLinkById(int id)
        {
            var link = await _service.GetLinkByIdAsync(id);
            if (link == null)
                return NotFound(ApiResult<PostTagMapResponse>.Failure(new[] { "Post or list Tag not found" }));
            return Ok(ApiResult<PostTagMapResponse>.Success(link));
        }

        [HttpPost]
        public async Task<IActionResult> CreateLink([FromBody] PostTagMapSaveDTO creatDTO)
        {
            var create = await _service.CreateLinkAsync(creatDTO);
            if (create == null)
                return BadRequest(ApiResult<PostTagMapResponse>.Failure(new[] { "Post or Tag not found" }));
            return Ok(ApiResult<PostTagMapResponse>.Success(create));
        }

        [HttpDelete("{postId}/{tagId}")]
        public async Task<IActionResult> DeleteLink(int postId, int tagId)
        {
            var delete = await _service.DeleteLinkAsync(postId, tagId);
            if (delete == null)
                return NotFound(ApiResult<PostTagMapResponse>.Failure(new[] { "Link does not exist" }));
            return Ok(ApiResult<PostTagMapResponse>.Success(delete));
        }
    }
}
