using Application.Models.Category.Response;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;

namespace Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostDTO>> GetAllPostAsync();
        Task<PostDTO?> GetByPostIdAsync(int id);
        Task<PostResponse> CreatePostAsync(PostSaveDTO createDTO, Guid userId);
        Task<PostResponse> UpdatePostAsync(int id, PostSaveDTO updateDTO);
        Task<PostResponse> DeletePostAsync(int id);
        Task<IEnumerable<PostResponse>> SearchAsync(string keyword);
    }
}
