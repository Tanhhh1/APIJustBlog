
using Application.Interfaces;
using Application.Models.Category.Response;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PostDTO>> GetAllPostAsync()
        {
            return _mapper.Map<IEnumerable<PostDTO>>(await _unitOfWork.PostRepository.GetAllAsync());
        }
        public async Task<PostDTO?> GetByPostIdAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if(post == null) return null;
            return _mapper.Map<PostDTO?>(post);
        }
        public async Task<PostResponse> CreatePostAsync(PostSaveDTO createDTO)
        {
            var create = _mapper.Map<Post>(createDTO);
            create.PostedOn = DateTime.UtcNow;
            await _unitOfWork.PostRepository.AddAsync(create);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PostResponse>(create);
        }
        public async Task<PostResponse> UpdatePostAsync(int id, PostSaveDTO updateDTO)
        {
            var update = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (update == null) return new PostResponse { Ok = false };
            _mapper.Map(updateDTO, update);
            update.Modified = DateTime.UtcNow;
            await _unitOfWork.PostRepository.UpdateAsync(update);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PostResponse>(update);
        }
        public async Task<PostResponse> DeletePostAsync(int id)
        {
            var delete = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (delete == null) return new PostResponse { Ok = false };
            await _unitOfWork.PostRepository.DeleteAsync(delete);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PostResponse>(delete);
        }
        public async Task<IEnumerable<PostResponse>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<PostResponse>();
            var categories = await _unitOfWork.PostRepository.SearchAsync(keyword);
            return _mapper.Map<IEnumerable<PostResponse>>(categories);
        }
    }
}
