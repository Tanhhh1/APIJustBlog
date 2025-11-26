using Application.DTOs.PostTagMap;
using Application.Interfaces;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;
using Shared.Logger;

namespace Application.Services
{
    public class PostTagMapService : IPostTagMapService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostTagMapService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PostTagMapResponse?> CreateLinkAsync(PostTagMapSaveDTO createDTO)
        {
            try
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(createDTO.PostId);
                if (post == null) return null;
                bool added = false;
                foreach (var id in createDTO.TagIds)
                {
                    var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
                    if (tag == null) continue;
                    // Kiểm tra xem liên kết post-tag đã tồn tại chưa
                    var maps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(createDTO.PostId);
                    if (maps.Any(x => x.TagId == id)) continue;
                    // Tạo liên kết mới và thêm vào repository
                    var link = new PostTagMap { PostId = createDTO.PostId, TagId = id };
                    await _unitOfWork.PostTagMapRepository.AddAsync(link);
                    added = true;
                }
                if (added)
                    await _unitOfWork.CompleteAsync();
                var list = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(createDTO.PostId);
                Logging.Info("Create link success for post {PostId}", createDTO.PostId);
                return _mapper.Map<PostTagMapResponse>(list);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error creating link for post");
                throw;
            }
        }
        public async Task<PostTagMapResponse?> GetLinkByIdAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
            {
                Logging.Warning("Post with ID {PostId} not found", id);
                return null;
            }
            var maps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(id);
            if (!maps.Any()) return null;
            return _mapper.Map<PostTagMapResponse>(maps);
        }
        public async Task<PostTagMapResponse?> DeleteLinkAsync(int postId, int tagId)
        {
            try
            {
                var p = await _unitOfWork.PostRepository.GetByIdAsync(postId);
                if (p == null) return null;
                var maps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(postId);
                // Tìm liên kết cần xóa
                var link = maps.FirstOrDefault(m => m.TagId == tagId);
                if (link == null) return null;
                await _unitOfWork.PostTagMapRepository.DeleteAsync(link);
                await _unitOfWork.CompleteAsync();
                var updatedMaps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(postId);
                Logging.Info("Delete link success for post {PostId} - tag {TagId}", postId, tagId);
                return _mapper.Map<PostTagMapResponse>(updatedMaps);
            }
            catch(Exception ex)
            {
                Logging.Error(ex, "Error deleting link for post {PostId} - tag {TagId}", postId, tagId);
                throw;
            }
        }
    }
}
