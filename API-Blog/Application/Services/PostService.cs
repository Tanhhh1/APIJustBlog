using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Logger;

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
            var posts = await _unitOfWork.PostRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PostDTO>>(posts);
        }
        public async Task<PostDTO?> GetByPostIdAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
            if (post == null)
            {
                Logging.Warning("Post with ID {PostId} not found", id);
                return null;
            }
            return _mapper.Map<PostDTO?>(post);
        }
        public async Task<PostResponse> CreatePostAsync(PostSaveDTO createDTO, Guid userId)
        {
            try
            {
                var isExist = await _unitOfWork.PostRepository.ExistsByUrlSlugAsync(createDTO.UrlSlug);
                if (isExist)
                {
                    Logging.Warning("Post create failed: UrlSlug '{UrlSlug}' already exists", createDTO.UrlSlug);
                    throw new BadRequestException("UrlSlug already exists.");
                }
                var create = _mapper.Map<Post>(createDTO);
                create.PostedOn = DateTime.UtcNow;
                await _unitOfWork.PostRepository.AddAsync(create);
                await _unitOfWork.CompleteAsync();

                Logging.Info("User {UserId} created post {PostId} successfully", userId, create.Id);
                return _mapper.Map<PostResponse>(create);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "User {UserId} failed to create post", userId);
                throw;
            }
        }
        public async Task<PostResponse> UpdatePostAsync(int id, PostSaveDTO updateDTO)
        {
            try
            {
                var isExist = await _unitOfWork.PostRepository.ExistsByUrlSlugAsync(updateDTO.UrlSlug);
                if (isExist)
                {
                    Logging.Warning("Post update failed: UrlSlug '{UrlSlug}' already exists", updateDTO.UrlSlug);
                    throw new BadRequestException("UrlSlug already exists.");
                }
                var update = await _unitOfWork.PostRepository.GetByIdAsync(id);
                if (update == null)
                    return new PostResponse { Ok = false };
                _mapper.Map(updateDTO, update);
                update.Modified = DateTime.UtcNow;
                await _unitOfWork.PostRepository.UpdateAsync(update);
                await _unitOfWork.CompleteAsync();

                Logging.Info("Updated post with ID {PostId} successfully", id);
                return _mapper.Map<PostResponse>(update);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error updating post with ID {PostId}", id);
                throw;
            }
        }
        public async Task<PostResponse> DeletePostAsync(int id)
        {
            try
            {
                var delete = await _unitOfWork.PostRepository.GetByIdAsync(id);
                if (delete == null)
                    return new PostResponse { Ok = false };

                await _unitOfWork.PostRepository.DeleteAsync(delete);
                await _unitOfWork.CompleteAsync();

                Logging.Info("Deleted post with ID {PostId} successfully", id);
                return _mapper.Map<PostResponse>(delete);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error deleting post with ID {PostId}", id);
                throw;
            }
        }
        public async Task<IEnumerable<PostResponse>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<PostResponse>();

            var posts = await _unitOfWork.PostRepository.SearchAsync(keyword);
            Logging.Info("Search for keyword '{Keyword}' returned {Count} results", keyword, posts.Count());
            return _mapper.Map<IEnumerable<PostResponse>>(posts);
        }
    }
}
