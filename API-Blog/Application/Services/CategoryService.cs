using Application.Common.ModelServices;
using Application.Interfaces;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PageList<CategoryDTO>> GetAllCateAsync(int pageNumber, int pageSize)
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            var count = categories.Count();

            if (pageSize <= 0)
            {
                pageSize = count == 0 ? 1 : count;
            }
            if (pageNumber < 1) pageNumber = 1;

            var pagedItems = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var dtoItems = _mapper.Map<IEnumerable<CategoryDTO>>(pagedItems);

            return new PageList<CategoryDTO>(dtoItems, count, pageNumber, pageSize);
        }

        public async Task<CategoryDTO?> GetByCateIdAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            return _mapper.Map<CategoryDTO?>(category);
        }
            
        public async Task<CategoryResponse> CreateCateAsync(CategorySaveDTO createDTO)
        {
            var create = _mapper.Map<Category>(createDTO);
            await _unitOfWork.CategoryRepository.AddAsync(create);
            await _unitOfWork.CompleteAsync();            
            return _mapper.Map<CategoryResponse>(create);
        }

        public async Task<CategoryResponse> UpdateCateAsync(int id, CategorySaveDTO updateDTO)
        {
            var update = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (update == null) return new CategoryResponse { Ok = false };
            _mapper.Map(updateDTO, update);
            await _unitOfWork.CategoryRepository.UpdateAsync(update);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryResponse>(update);
        }

        public async Task<CategoryResponse> DeleteCateAsync(int id)
        {
            var delete = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if(delete == null) return new CategoryResponse { Ok = false };
            await _unitOfWork.CategoryRepository.DeleteAsync(delete);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CategoryResponse>(delete);
        }

        public async Task<IEnumerable<CategoryResponse>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<CategoryResponse>();
            var categories = await _unitOfWork.CategoryRepository.SearchAsync(keyword);
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }
    }
}
