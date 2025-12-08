using Application.DTOs.PostTagMap;

namespace Application.Interfaces.Services
{
    public interface IPostTagMapService
    {
        Task<PostTagMapResponse?> CreateLinkAsync(PostTagMapSaveDTO createDTO);
        Task<PostTagMapResponse?> GetLinkByIdAsync(int id);
        Task<PostTagMapResponse?> DeleteLinkAsync(int postId, int tagId);
    }
}
