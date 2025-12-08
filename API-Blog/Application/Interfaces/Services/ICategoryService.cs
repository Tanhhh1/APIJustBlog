using Application.Common.ModelServices;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;

namespace Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<PageList<CategoryDTO>> GetAllCateAsync(int pageNumber, int pageSize);
        Task<CategoryDTO?> GetByCateIdAsync(int id);
        Task<CategoryResponse> CreateCateAsync(CategorySaveDTO createDTO);
        Task<CategoryResponse> UpdateCateAsync(int id, CategorySaveDTO updateDTO);
        Task<CategoryResponse> DeleteCateAsync(int id);
        Task<IEnumerable<CategoryResponse>> SearchAsync(string keyword);
    }
}
