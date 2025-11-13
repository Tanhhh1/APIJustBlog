using Application.DTOs.PostTagMap;
using Application.Interfaces;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;

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
            var post = await _unitOfWork.PostRepository.GetByIdAsync(createDTO.PostId);
            if (post == null) return null; // Nếu post không tồn tại, trả về null
            bool added = false;
            foreach (var id in createDTO.TagIds)
            {
                var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
                if (tag == null) continue; // Bỏ qua nếu tag không tồn tại
                // Kiểm tra xem liên kết post-tag đã tồn tại chưa
                var maps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(createDTO.PostId);
                if (maps.Any(x => x.TagId == id)) continue;
                // Tạo liên kết mới và thêm vào repository
                var link = new PostTagMap { PostId = createDTO.PostId, TagId = id };
                await _unitOfWork.PostTagMapRepository.AddAsync(link);
                added = true;
            }
            // Nếu có liên kết mới được thêm, lưu thay đổi vào DB
            if (added)
                await _unitOfWork.CompleteAsync();
            // Lấy danh sách liên kết mới nhất để trả về response
            var list = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(createDTO.PostId);
            // Map entity sang response DTO
            return _mapper.Map<PostTagMapResponse>(list);
        }
        public async Task<PostTagMapResponse?> GetLinkByIdAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null) return null; // Nếu post không tồn tại, trả về null
            var maps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(id);
            if (!maps.Any()) return null; // Nếu post chưa có tag nào
            return _mapper.Map<PostTagMapResponse>(maps);
        }
        public async Task<PostTagMapResponse?> DeleteLinkAsync(int postId, int tagId)
        {
            var p = await _unitOfWork.PostRepository.GetByIdAsync(postId);
            if (p == null) return null; // Nếu post không tồn tại
            var maps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(postId);
            // Tìm liên kết cần xóa
            var link = maps.FirstOrDefault(m => m.TagId == tagId);
            if (link == null) return null; // Nếu liên kết không tồn tại
            await _unitOfWork.PostTagMapRepository.DeleteAsync(link); // Xóa liên kết
            await _unitOfWork.CompleteAsync(); // Lưu thay đổi
            // Lấy danh sách liên kết còn lại và trả về response
            var updatedMaps = await _unitOfWork.PostTagMapRepository.GetByPostIdAsync(postId);
            return _mapper.Map<PostTagMapResponse>(updatedMaps);
        }
    }
}
