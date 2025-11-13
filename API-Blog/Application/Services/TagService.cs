using Application.Interfaces;
using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;

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
            if(tag == null) return null;
            return _mapper.Map<TagDTO?>(tag); 
        }

        public async Task<TagResponse> CreateTagAsync(TagSaveDTO createDTO)
        {
            var create = _mapper.Map<Tag>(createDTO);
            await _unitOfWork.TagRepository.AddAsync(create);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TagResponse>(create);
        }

        public async Task<TagResponse> UpdateTagAsync(int id, TagSaveDTO updateDTO)
        {
            var update = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (update == null) return new TagResponse { Ok = false };
            _mapper.Map(updateDTO, update);
            await _unitOfWork.TagRepository.UpdateAsync(update);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TagResponse>(update);
        }

        public async Task<TagResponse> DeleteTagAsync(int id)
        {
            var delete = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (delete == null) return new TagResponse { Ok = false };
            await _unitOfWork.TagRepository.DeleteAsync(delete);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TagResponse>(delete);
        }
    }
}
