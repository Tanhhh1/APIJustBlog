using Application.Interfaces.UnitOfWork;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Moq;
using Test.Common;

namespace Test.Application.Services
{
    public class PostServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IPostRepository> _mockPostRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly PostService _postService;
        private readonly Guid _userId = Guid.NewGuid();

        public PostServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperTestProfile());
            });

            _mapper = mapperConfig.CreateMapper();
            _mockPostRepo = new Mock<IPostRepository>();
            _mockUow = new Mock<IUnitOfWork>();

            _mockUow.Setup(u => u.PostRepository).Returns(_mockPostRepo.Object);
            _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _postService = new PostService(_mockUow.Object, _mapper);
        }

        [Fact]
        public async Task GetAllPostAsync_ReturnAllPosts()
        {
            _mockPostRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(
                new List<Post>
                {
                    TestDataFactory.CreatePost(1),
                    TestDataFactory.CreatePost(2)
                });
            var result = await _postService.GetAllPostAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllPostAsync_ReturnEmpty_NoPost()
        {
            _mockPostRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Post>());
            var result = await _postService.GetAllPostAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public async Task GetByPostIdAsync_TestVariousCases(int id, bool exists)
        {
            if (exists)
            {
                var post = TestDataFactory.CreatePost(id);
                _mockPostRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(post);
                var result = await _postService.GetByPostIdAsync(id);
                Assert.NotNull(result);
            }
            else
            {
                _mockPostRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Post)null);
                var result = await _postService.GetByPostIdAsync(id);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreatePostAsync_ReturnCreatedResponse()
        {
            var dto = TestDataFactory.CreatePostSaveDTO();
            _mockPostRepo.Setup(r => r.AddAsync(It.IsAny<Post>()));
            var result = await _postService.CreatePostAsync(dto, _userId);
            Assert.NotNull(result);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePostAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreatePostSaveDTO();
            _mockPostRepo.Setup(r => r.AddAsync(It.IsAny<Post>()))
                         .ThrowsAsync(new Exception("Add failed"));
            await Assert.ThrowsAsync<Exception>(() => _postService.CreatePostAsync(dto, _userId));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Theory]
        [InlineData(1, "Title 1", true)]
        [InlineData(2, "Title 2", false)]
        public async Task UpdatePostAsync_TestCases(int id, string title, bool exists)
        {
            if (exists)
            {
                var existing = TestDataFactory.CreatePost(id);
                _mockPostRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                var dto = TestDataFactory.CreatePostSaveDTO(title);
                var result = await _postService.UpdatePostAsync(id, dto);
                Assert.True(result.Ok);
                Assert.True(existing.Modified <= DateTime.UtcNow);
                _mockPostRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
            }
            else
            {
                _mockPostRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Post?)null);
                var dto = TestDataFactory.CreatePostSaveDTO(title);
                var result = await _postService.UpdatePostAsync(id, dto);
                Assert.False(result.Ok);
                _mockPostRepo.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Never);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
            }
        }

        [Fact]
        public async Task UpdatePostAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreatePostSaveDTO();
            _mockPostRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                         .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<Exception>(() => _postService.UpdatePostAsync(1, dto));
            _mockPostRepo.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Never);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, false)]
        public async Task DeletePostAsync_TestCases(int id, bool exists)
        {
            if (exists)
            {
                var existing = TestDataFactory.CreatePost(id);
                _mockPostRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
                var result = await _postService.DeletePostAsync(id);
                Assert.True(result.Ok);
                _mockPostRepo.Verify(r => r.DeleteAsync(existing), Times.Once);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
            }
            else
            {
                _mockPostRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Post)null);
                var result = await _postService.DeletePostAsync(id);
                Assert.False(result.Ok);
                _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
            }
        }

        [Fact]
        public async Task DeletePostAsync_ExceptionPropagates()
        {
            var existing = TestDataFactory.CreatePost(1);
            _mockPostRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockPostRepo.Setup(r => r.DeleteAsync(existing)).ThrowsAsync(new Exception("Delete failed"));
            await Assert.ThrowsAsync<Exception>(() => _postService.DeletePostAsync(1));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Theory]
        [InlineData("a", 2)]
        [InlineData("xyz", 0)]
        public async Task SearchAsync_TestCases(string keyword, int expectedCount)
        {
            var list = expectedCount == 2
                ? new List<Post>
                {
                    TestDataFactory.CreatePost(1, "Apple Post"),
                    TestDataFactory.CreatePost(2, "Banana Post")
                }
                : new List<Post>();
            _mockPostRepo.Setup(r => r.SearchAsync(keyword)).ReturnsAsync(list);
            var result = await _postService.SearchAsync(keyword);
            Assert.Equal(expectedCount, result.Count());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchAsync_KeywordInvalid_ReturnsEmpty(string keyword)
        {
            var result = await _postService.SearchAsync(keyword);
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockPostRepo.Verify(r => r.SearchAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
