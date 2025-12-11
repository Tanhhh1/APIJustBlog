using Application.Interfaces.UnitOfWork;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Moq;
using Test.Common;
using Application.Interfaces.Security;

namespace Test.Application.Services
{
    public class CategoryServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ICategoryRepository> _mockCategoryRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IEncryptionService> _mockEncrypt;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperTestProfile());
            });
            _mapper = mapperConfig.CreateMapper();
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockEncrypt = new Mock<IEncryptionService>();
            _mockUow.Setup(u => u.CategoryRepository).Returns(_mockCategoryRepo.Object);
            _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _categoryService = new CategoryService(_mockUow.Object, _mapper, _mockEncrypt.Object);
        }

        [Fact]
        public async Task GetAllCateAsync_ReturnPagedResult()
        {
            _mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
                new List<Category> {
                    TestDataFactory.CreateCategory(1),
                    TestDataFactory.CreateCategory(2),
                });
            var result = await _categoryService.GetAllCateAsync(1, 2);
            Assert.Equal(2, result.TotalCount);
        }
        [Fact]
        public async Task GetAllCateAsync_ReturnEmpty_NoCategory()
        {
            _mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());
            var result = await _categoryService.GetAllCateAsync(1, 2);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public async Task GetByCateIdAsync_TestVariousCases(int id, bool exists)
        {
            if (exists)
            {
                var category = TestDataFactory.CreateCategory(id);
                _mockCategoryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);
                var result = await _categoryService.GetByCateIdAsync(id);
                Assert.NotNull(result);
            }
            else
            {
                _mockCategoryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category)null);
                var result = await _categoryService.GetByCateIdAsync(id);
                Assert.Null(result);
            }
        }
        [Fact]
        public async Task CreateCateAsync_ReturnCreatedResponse()
        {
            var dto = TestDataFactory.CreateCategorySaveDTO();
            _mockCategoryRepo.Setup(r => r.AddAsync(It.IsAny<Category>()));
            var result = await _categoryService.CreateCateAsync(dto);
            Assert.NotNull(result);
            _mockCategoryRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task CreateCateAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreateCategorySaveDTO();
            _mockCategoryRepo.Setup(r => r.AddAsync(It.IsAny<Category>()))
                             .ThrowsAsync(new Exception("Add failed"));
            await Assert.ThrowsAsync<Exception>(() => _categoryService.CreateCateAsync(dto));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Theory]
        [InlineData(1, "Name 1", true)]
        [InlineData(2, "Name 2", false)]
        public async Task UpdateCateAsync_TestCases(int id, string name, bool exists)
        {
            if (exists)
            {
                var existing = TestDataFactory.CreateCategory(id);
                _mockCategoryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                var dto = TestDataFactory.CreateCategorySaveDTO(name);
                var result = await _categoryService.UpdateCateAsync(id, dto);
                Assert.True(result.Ok);
                _mockCategoryRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
            }
            else
            {
                _mockCategoryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category)null);
                var dto = TestDataFactory.CreateCategorySaveDTO(name);
                var result = await _categoryService.UpdateCateAsync(id, dto);
                Assert.False(result.Ok);
                _mockCategoryRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
            }
        }
        [Fact]
        public async Task UpdateCateAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreateCategorySaveDTO();
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                         .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<Exception>(() => _categoryService.UpdateCateAsync(1, dto));
            _mockCategoryRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public async Task DeleteCateAsync_TestCases(int id, bool exists)
        {
            if (exists)
            {
                var existing = TestDataFactory.CreateCategory(id);
                _mockCategoryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                var result = await _categoryService.DeleteCateAsync(id);
                Assert.True(result.Ok);
                _mockCategoryRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
            }
            else
            {
                _mockCategoryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);
                var result = await _categoryService.DeleteCateAsync(id);
                Assert.False(result.Ok);
                _mockCategoryRepo.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
            }
        }
        [Fact]
        public async Task DeletePostAsync_ExceptionPropagates()
        {
            var existing = TestDataFactory.CreateCategory(1);
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockCategoryRepo.Setup(r => r.DeleteAsync(existing)).ThrowsAsync(new Exception("Delete failed"));
            await Assert.ThrowsAsync<Exception>(() => _categoryService.DeleteCateAsync(1));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Theory]
        [InlineData("a", 2)]
        [InlineData("xyz", 0)]
        public async Task SearchAsync_ValidKeyword_ReturnsExpected(string keyword, int expectedCount)
        {
            var list = expectedCount == 2
                ? new List<Category> {
                    TestDataFactory.CreateCategory(1, "Apple"),
                    TestDataFactory.CreateCategory(2, "Banana")
                }
                : new List<Category>();
            _mockCategoryRepo.Setup(r => r.SearchAsync(keyword)).ReturnsAsync(list);
            var result = await _categoryService.SearchAsync(keyword);
            Assert.Equal(expectedCount, result.Count());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchAsync_InvalidKeyword_ReturnsEmpty(string keyword)
        {
            var result = await _categoryService.SearchAsync(keyword);
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCategoryRepo.Verify(r => r.SearchAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
