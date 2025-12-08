using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;

namespace Application.Interfaces.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTagAsync();
        Task<TagDTO?> GetByTagIdAsync(int id);
        Task<TagResponse> CreateTagAsync(TagSaveDTO createDTO);
        Task<TagResponse> UpdateTagAsync(int id, TagSaveDTO updateDTO);
        Task<TagResponse> DeleteTagAsync(int id);
    }
}
