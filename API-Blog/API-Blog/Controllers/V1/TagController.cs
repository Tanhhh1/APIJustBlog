using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces.Services;
using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Blog.Controllers.V1
{
    [EnableRateLimiting("CrudPolicy")]
    public class TagController : ApiController
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTag()
        {
            return Ok(ApiResult<IEnumerable<TagDTO>>.Success(await _tagService.GetAllTagAsync()));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByTagId(int id)
        {
            var tag = await _tagService.GetByTagIdAsync(id);
            if (tag == null)
                return NotFound(ApiResult<TagDTO>.Failure(new List<string> { "Tag not found" }));
            return Ok(ApiResult<TagDTO>.Success(tag));
        }
        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] TagSaveDTO createDTO)
        {
            var create = await _tagService.CreateTagAsync(createDTO);
            return Ok(ApiResult<TagResponse>.Success(create));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag([FromBody] TagSaveDTO updateDTO, int id)
        {
            var update = await _tagService.UpdateTagAsync(id, updateDTO);
            if (!update.Ok)
                return NotFound(ApiResult<TagResponse>.Failure(new List<string> { "Tag not found" }));
            return Ok(ApiResult<TagResponse>.Success(update));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var delete = await _tagService.DeleteTagAsync(id);
            if (!delete.Ok)
                return NotFound(ApiResult<TagResponse>.Failure(new List<string> { "Tag not found" }));
            return Ok(ApiResult<TagResponse>.Success(delete));
        }
    }
}
