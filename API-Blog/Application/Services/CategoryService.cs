using Application.Common.ModelServices;
using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Application.Interfaces.UnitOfWork;
using AutoMapper;
using Domain.Entities;
using Shared.Logger;

namespace Application.Services
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
            if (category == null)
            {
                Logging.Warning("Category with ID {CategoryId} not found", id);
                return null;
            }
            return _mapper.Map<CategoryDTO?>(category);
        }  
        public async Task<CategoryResponse> CreateCateAsync(CategorySaveDTO createDTO)
        {
            try
            {
                var isExist = await _unitOfWork.CategoryRepository.ExistsByUrlSlugAsync(createDTO.UrlSlug);
                if (isExist)
                {
                    Logging.Warning("Category create failed: UrlSlug '{UrlSlug}' already exists", createDTO.UrlSlug);
                    throw new BadRequestException("UrlSlug already exists.");
                }
                var create = _mapper.Map<Category>(createDTO);
                await _unitOfWork.CategoryRepository.AddAsync(create);
                await _unitOfWork.CompleteAsync();

                Logging.Info("Category {CategoryId} is created successfully", create.Id);
                return _mapper.Map<CategoryResponse>(create);
            }
            catch (Exception ex) 
            {
                Logging.Error(ex, "Error creating category");
                throw;
            }
        }
        public async Task<CategoryResponse> UpdateCateAsync(int id, CategorySaveDTO updateDTO)
        {
            try
            {
                var isExist = await _unitOfWork.CategoryRepository.ExistsByUrlSlugAsync(updateDTO.UrlSlug);
                if (isExist)
                {
                    Logging.Warning("Category update failed: UrlSlug '{UrlSlug}' already exists", updateDTO.UrlSlug);
                    throw new BadRequestException("UrlSlug already exists.");
                }
                var update = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (update == null) return new CategoryResponse { Ok = false };
                _mapper.Map(updateDTO, update);
                await _unitOfWork.CategoryRepository.UpdateAsync(update);
                await _unitOfWork.CompleteAsync();

                Logging.Info("Updated category with ID {CategoryId} successfully", id);
                return _mapper.Map<CategoryResponse>(update);
            }
            catch (Exception ex) 
            {
                Logging.Error(ex, "Error updating category with ID {CategoryId}", id);
                throw;
            }
        }
        public async Task<CategoryResponse> DeleteCateAsync(int id)
        {
            try
            {
                var delete = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (delete == null) return new CategoryResponse { Ok = false };
                await _unitOfWork.CategoryRepository.DeleteAsync(delete);
                await _unitOfWork.CompleteAsync();

                Logging.Info("Deleted category with ID {CategoryId} successfully", id);
                return _mapper.Map<CategoryResponse>(delete); 
            }
            catch (Exception ex) 
            {
                Logging.Error(ex, "Error deleting category with ID {CategoryId}", id);
                throw;
            }
        }
        public async Task<IEnumerable<CategoryResponse>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<CategoryResponse>();
            var categories = await _unitOfWork.CategoryRepository.SearchAsync(keyword);
            Logging.Info("Search for keyword '{Keyword}' returned {Count} results", keyword, categories.Count());
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }
    }
}
