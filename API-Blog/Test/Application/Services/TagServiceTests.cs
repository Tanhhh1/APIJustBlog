using Application.Interfaces.UnitOfWork;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Moq;
using Test.Common;


namespace Test.Application.Services
{
    public class TagServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ITagRepository> _mockTagRepo;
        private readonly TagService _tagService;

        public TagServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperTestProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
            _mockUow = new Mock<IUnitOfWork>();
            _mockTagRepo = new Mock<ITagRepository>();
            _mockUow.Setup(u => u.TagRepository).Returns(_mockTagRepo.Object);
            _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
            _tagService = new TagService(_mockUow.Object, _mapper);
        }
        [Fact]
        public async Task GetAllTagAsync_ReturnAllTags()
        {
            _mockTagRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
                new List<Tag>
                {
                    TestDataFactory.CreateTag(1),
                    TestDataFactory.CreateTag(2)
                });
            var result = await _tagService.GetAllTagAsync();
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public async Task GetAllTagAsync_ReturnEmpty_NoTag()
        {
            _mockTagRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Tag>());
            var result = await _tagService.GetAllTagAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public async Task GetByTagIdAsync_TestVariousCases(int id, bool exists)
        {
            if (exists)
            {
                var tag = TestDataFactory.CreateTag(id);
                _mockTagRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(tag);
                var result = await _tagService.GetByTagIdAsync(id);
                Assert.NotNull(result);
            }
            else
            {
                _mockTagRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Tag?)null);
                var result = await _tagService.GetByTagIdAsync(id);
                Assert.Null(result);
            }
        }
        [Fact]
        public async Task CreateTagAsync_ReturnCreatedResponse()
        {
            var dto = TestDataFactory.CreateTagSaveDTO();
            _mockTagRepo.Setup(r => r.AddAsync(It.IsAny<Tag>()));
            var result = await _tagService.CreateTagAsync(dto);
            Assert.NotNull(result);
            _mockTagRepo.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Once);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
        }
        [Fact]
        public async Task CreateCateAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreateTagSaveDTO();
            _mockTagRepo.Setup(r => r.AddAsync(It.IsAny<Tag>()))
                             .ThrowsAsync(new Exception("Add failed"));
            await Assert.ThrowsAsync<Exception>(() => _tagService.CreateTagAsync(dto));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }
        [Theory]
        [InlineData(1, "New Name", true)]
        [InlineData(2, "New Name", false)]
        public async Task UpdateTagAsync_TestCases(int id, string name, bool exists)
        {
            if (exists)
            {
                var existing = TestDataFactory.CreateTag(id, "Old Name");
                _mockTagRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                var dto = TestDataFactory.CreateTagSaveDTO(name);
                var result = await _tagService.UpdateTagAsync(id, dto);
                Assert.True(result.Ok);
                _mockTagRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
            }
            else
            {
                _mockTagRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Tag?)null);
                var dto = TestDataFactory.CreateTagSaveDTO(name);
                var result = await _tagService.UpdateTagAsync(id, dto);
                Assert.False(result.Ok);
                _mockTagRepo.Verify(r => r.UpdateAsync(It.IsAny<Tag>()), Times.Never);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
            }
        }
        [Fact]
        public async Task UpdateTagAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreateTagSaveDTO();
            _mockTagRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                         .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<Exception>(() => _tagService.UpdateTagAsync(1, dto));
            _mockTagRepo.Verify(r => r.UpdateAsync(It.IsAny<Tag>()), Times.Never);
        }
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public async Task DeleteTagAsync_TestCases(int id, bool exists)
        {
            if (exists)
            {
                var existing = TestDataFactory.CreateTag(id);
                _mockTagRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                var result = await _tagService.DeleteTagAsync(id);
                Assert.True(result.Ok);
                _mockTagRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
            }
            else
            {
                _mockTagRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Tag?)null);
                var result = await _tagService.DeleteTagAsync(id);
                Assert.False(result.Ok);
                _mockTagRepo.Verify(r => r.DeleteAsync(It.IsAny<Tag>()), Times.Never);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
            }
        }
        [Fact]
        public async Task DeletePostAsync_ExceptionPropagates()
        {
            var existing = TestDataFactory.CreateTag(1);
            _mockTagRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockTagRepo.Setup(r => r.DeleteAsync(existing)).ThrowsAsync(new Exception("Delete failed"));
            await Assert.ThrowsAsync<Exception>(() => _tagService.DeleteTagAsync(1));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }
    }
}
