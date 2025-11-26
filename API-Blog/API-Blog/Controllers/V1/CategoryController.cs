using API_Blog.Controllers.Common;
using Application.Common.ModelServices;
using Application.Interfaces;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API_Blog.Controllers.V1
{
    public class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCate([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string keyword = null)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var result = await _categoryService.SearchAsync(keyword);
                if (!result.Any())
                    return NotFound(ApiResult<CategoryResponse>.Failure(new List<string> { "No matching categories found" }));
                return Ok(result);
            }
            var paged = await _categoryService.GetAllCateAsync(pageNumber, pageSize);
            return Ok(ApiResult<PageList<CategoryDTO>>.Success(paged));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByCateId(int id)
        {
            var category = await _categoryService.GetByCateIdAsync(id);
            if (category == null) //khi service trả nullable DTO / Entity
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
            if (!update.Ok) //khi service trả response object với property Ok
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
