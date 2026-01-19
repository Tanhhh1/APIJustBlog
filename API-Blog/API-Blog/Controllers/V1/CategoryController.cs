using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces.Services;
using Application.Models.Category;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_Blog.Controllers.V1
{
    [EnableRateLimiting("CrudPolicy")]
    public class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CategoryQuery query)
        {
            var result = await _categoryService.GetAllCateAsync(query);
            return Ok(ApiResult<PageList<CategoryDTO>>.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByCateId(int id)
        {
            var category = await _categoryService.GetByCateIdAsync(id);
            if (category == null)
                return NotFound(ApiResult<CategoryDTO>.Failure(new List<string> { "Category not found" }));
            return Ok(ApiResult<CategoryDTO>.Success(category));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCate([FromBody] CategorySaveDTO createDTO)
        {
            var create = await _categoryService.CreateCateAsync(createDTO);
            return Ok(ApiResult<CategoryResponse>.Success(create));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCate([FromBody] CategorySaveDTO updateDTO, int id)
        {
            var update = await _categoryService.UpdateCateAsync(id, updateDTO);
            if (!update.Ok)
                return NotFound(ApiResult<CategoryResponse>.Failure(new List<string> { "Category not found" }));
            return Ok(ApiResult<CategoryResponse>.Success(update));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCate(int id)
        {
            var delete = await _categoryService.DeleteCateAsync(id);
            if (!delete.Ok)
                return NotFound(ApiResult<CategoryResponse>.Failure(new List<string> { "Category not found" }));
            return Ok(ApiResult<CategoryResponse>.Success(delete));
        }
    }
}
