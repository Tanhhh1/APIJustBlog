using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.UnitOfWork;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Test.Application.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<ICategoryRepository> _repo;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IEncryptionService> _crypto;

        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _repo = new Mock<ICategoryRepository>();
            _uow = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _crypto = new Mock<IEncryptionService>();

            _uow.Setup(u => u.CategoryRepository).Returns(_repo.Object);
            _uow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _service = new CategoryService(
                _uow.Object,
                _mapper.Object,
                _crypto.Object
            );
        }

        [Fact]
        public async Task GetAllCateAsync_Success()
        {
            var categories = new List<Category>
            {
                new() { UrlSlug = "enc1" },
                new() { UrlSlug = "enc2" }
            };
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
            _crypto.Setup(c => c.Decrypt(It.IsAny<string>())).Returns("dec");
            _mapper.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>()))
                   .Returns(new List<CategoryDTO> { new(), new() });

            var result = await _service.GetAllCateAsync(1, 10);

            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task GetAllCateAsync_Exception_ThrowsBadRequest()
        {
            _repo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetAllCateAsync(1, 10)
            );
        }

        [Fact]
        public async Task GetByCateIdAsync_Found()
        {
            var category = new Category { UrlSlug = "enc" };
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _crypto.Setup(c => c.Decrypt("enc")).Returns("dec");
            _mapper.Setup(m => m.Map<CategoryDTO>(category))
                   .Returns(new CategoryDTO());
            var result = await _service.GetByCateIdAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByCateIdAsync_NotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category)null);
            var result = await _service.GetByCateIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCateAsync_UrlSlugExists_Throws()
        {
            var dto = new CategorySaveDTO { UrlSlug = "slug" };
            _repo.Setup(r => r.ExistsByUrlSlugAsync("slug")).ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.CreateCateAsync(dto)
            );
        }

        [Fact]
        public async Task CreateCateAsync_Valid_CreatesCategory()
        {
            var dto = new CategorySaveDTO { UrlSlug = "slug" };
            var category = new Category { UrlSlug = "enc" };

            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<Category>(dto)).Returns(category);
            _crypto.Setup(c => c.Encrypt(dto.UrlSlug)).Returns("enc");
            _crypto.Setup(c => c.Decrypt("enc")).Returns("dec");
            _mapper.Setup(m => m.Map<CategoryResponse>(category))
                   .Returns(new CategoryResponse { Ok = true });

            var result = await _service.CreateCateAsync(dto);

            Assert.True(result.Ok);
            _repo.Verify(r => r.AddAsync(category), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCateAsync_NotFound_ReturnsOkFalse()
        {
            _repo.Setup(r => r.ExistsByUrlSlugAsync(It.IsAny<string>()))
                 .ReturnsAsync(false);
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category)null);
            var result = await _service.UpdateCateAsync(1, new CategorySaveDTO());
            Assert.False(result.Ok);
        }

        [Fact]
        public async Task UpdateCateAsync_UrlSlugExists_Throws()
        {
            var dto = new CategorySaveDTO { UrlSlug = "slug" };
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug))
                 .ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.UpdateCateAsync(1, dto)
            );
            _repo.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
            _uow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateCateAsync_Valid_UpdateSuccess()
        {
            var dto = new CategorySaveDTO{ UrlSlug = "new-slug" };
            var category = new Category();
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug)).ReturnsAsync(false);
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
            _crypto.Setup(c => c.Encrypt(dto.UrlSlug)).Returns("new-enc");
            _crypto.Setup(c => c.Decrypt("new-enc")).Returns("new-slug");
            _mapper.Setup(m => m.Map(dto, category));
            _mapper.Setup(m => m.Map<CategoryResponse>(category))
                   .Returns(new CategoryResponse { Ok = true });
            var result = await _service.UpdateCateAsync(1, dto);
            Assert.True(result.Ok);
            _repo.Verify(r => r.UpdateAsync(category), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCateAsync_NotFound_ReturnsOkFalse()
        {
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category)null);
            var result = await _service.DeleteCateAsync(1);
            Assert.False(result.Ok);
        }

        [Fact]
        public async Task DeleteCateAsync_Exception_ThrowsBadRequest()
        {
            var category = new Category { Id = 1 };
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(category);
            _repo.Setup(r => r.DeleteAsync(category))
                 .ThrowsAsync(new Exception("Delete failed"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.DeleteCateAsync(1)
            );
            _uow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteCateAsync_Valid_DeleteSuccess()
        {
            var category = new Category { Id = 1 };
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(category);

            _mapper.Setup(m => m.Map<CategoryResponse>(category))
                   .Returns(new CategoryResponse { Ok = true });
            var result = await _service.DeleteCateAsync(1);
            Assert.True(result.Ok);
            _repo.Verify(r => r.DeleteAsync(category), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }


        [Fact]
        public async Task SearchAsync_EmptyKeyword_ReturnsEmpty()
        {
            var result = await _service.SearchAsync("");
            Assert.Empty(result);
            _repo.Verify(r => r.SearchAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchAsync_ValidKeyword_ReturnsResults()
        {
            var categories = new List<Category>
            {
                new Category(),
                new Category()
            };
            _repo.Setup(r => r.SearchAsync("test"))
                 .ReturnsAsync(categories);
            _mapper.Setup(m => m.Map<IEnumerable<CategoryResponse>>(categories))
                   .Returns(new List<CategoryResponse>{ new(), new() });
            var result = await _service.SearchAsync("test");
            Assert.Equal(2, result.Count());
        }
    }
}
