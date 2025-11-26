using Application.Interfaces;
using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;
using Shared.Logger;

namespace Application.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<TagDTO>> GetAllTagAsync()
        {
            return _mapper.Map<IEnumerable<TagDTO>>(await _unitOfWork.TagRepository.GetAllAsync());
        }
        public async Task<TagDTO?> GetByTagIdAsync(int id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if(tag == null)
            {
                Logging.Warning("Tag with ID {TagId} not found", id);
                return null;
            }
            return _mapper.Map<TagDTO?>(tag); 
        }
        public async Task<TagResponse> CreateTagAsync(TagSaveDTO createDTO)
        {
            try
            {
                var create = _mapper.Map<Tag>(createDTO);
                await _unitOfWork.TagRepository.AddAsync(create);
                await _unitOfWork.CompleteAsync();
                Logging.Info("Tag {TagId} is created successfully", create.Id);
                return _mapper.Map<TagResponse>(create);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error creating tag");
                throw;
            }
        }
        public async Task<TagResponse> UpdateTagAsync(int id, TagSaveDTO updateDTO)
        {
            try
            {
                var update = await _unitOfWork.TagRepository.GetByIdAsync(id);
                if (update == null) return new TagResponse { Ok = false };
                _mapper.Map(updateDTO, update);
                await _unitOfWork.TagRepository.UpdateAsync(update);
                await _unitOfWork.CompleteAsync();
                Logging.Info("Updated tag with ID {TagId} successfully", id);
                return _mapper.Map<TagResponse>(update);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error updating tag with ID {TagId}", id);
                throw;
            }
        }
        public async Task<TagResponse> DeleteTagAsync(int id)
        {
            try
            {
                var delete = await _unitOfWork.TagRepository.GetByIdAsync(id);
                if (delete == null) return new TagResponse { Ok = false };
                await _unitOfWork.TagRepository.DeleteAsync(delete);
                await _unitOfWork.CompleteAsync();
                Logging.Info("Deleted tag with ID {TagId} successfully", id);
                return _mapper.Map<TagResponse>(delete);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error deleting tag with ID {TagId}", id);
                throw;
            }
        }
    }
}
