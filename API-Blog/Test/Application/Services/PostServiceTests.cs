using Application.Exceptions;
using Application.Interfaces.Caching;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Test.Application.Services
{
    public class PostServiceTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostRepository> _repo;
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<ICacheService> _cache;
        private readonly PostService _service;
        private readonly Guid _userId = Guid.NewGuid();

        public PostServiceTests()
        {
            _repo = new Mock<IPostRepository>();
            _uow = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _cache = new Mock<ICacheService>();

            _uow.Setup(u => u.PostRepository).Returns(_repo.Object);
            _uow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _service = new PostService(
                _uow.Object,
                _mapper.Object,
                _cache.Object
            );
        }

        [Fact]
        public async Task GetAllPostsAsync_CacheMiss()
        {
            var posts = new List<Post>
            {
                new() { Title = "Post 1" },
                new() { Title = "Post 2" }
            };
            _cache.Setup(c => c.GetAsync<IEnumerable<PostDTO>>(It.IsAny<string>()))
                  .ReturnsAsync((IEnumerable<PostDTO>?)null);

            _repo.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(posts);

            _mapper.Setup(m => m.Map<IEnumerable<PostDTO>>(posts))
                   .Returns(new List<PostDTO> { new(), new()});
            var result = await _service.GetAllPostAsync();

            Assert.Equal(2, result.Count());

            _repo.Verify(r => r.GetAllAsync(), Times.Once);
            _cache.Verify(c =>
                c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<PostDTO>>(),
                    It.IsAny<TimeSpan>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAllPostsAsync_CacheHit()
        {
            _cache.Setup(c => c.GetAsync<IEnumerable<PostDTO>>(It.IsAny<string>()))
                  .ReturnsAsync(new List<PostDTO> { new(), new() });
            var result = await _service.GetAllPostAsync();

            Assert.Equal(2, result.Count());

            _repo.Verify(r => r.GetAllAsync(), Times.Never);
            _mapper.Verify(
                m => m.Map<IEnumerable<PostDTO>>(It.IsAny<IEnumerable<Post>>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetAllPostsAsync_Exception_Throws()
        {
            _cache.Setup(c => c.GetAsync<IEnumerable<PostDTO>>(It.IsAny<string>()))
                  .ReturnsAsync((IEnumerable<PostDTO>?)null);
            _repo.Setup(r => r.GetAllAsync())
                 .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetAllPostAsync()
            );
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetByPostIdAsync_ReturnsExpected(bool found)
        {
            var post = new Post();
            var dto = new PostDTO();
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _mapper.Setup(m => m.Map<PostDTO>(post)).Returns(dto);
            var result = await _service.GetByPostIdAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreatePostAsync_UrlSlugExists_Throws()
        {
            var dto = new PostSaveDTO { UrlSlug = "slug" };
            _repo.Setup(r => r.ExistsByUrlSlugAsync("slug"))
                 .ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.CreatePostAsync(dto, _userId));
        }

        [Fact]
        public async Task CreatePostAsync_Valid_CreatesPost()
        {
            var dto = new PostSaveDTO { UrlSlug = "slug" };
            var post = new Post();
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug))
                 .ReturnsAsync(false);
            _mapper.Setup(m => m.Map<Post>(dto)).Returns(post);
            _mapper.Setup(m => m.Map<PostResponse>(post))
                   .Returns(new PostResponse { Ok = true });
            var result = await _service.CreatePostAsync(dto, _userId);
            Assert.True(result.Ok);
            _repo.Verify(r => r.AddAsync(post), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_UrlSlugExists_Throws()
        {
            var dto = new PostSaveDTO { UrlSlug = "slug" };
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug))
                 .ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.UpdatePostAsync(1, dto));
        }

        [Fact]
        public async Task UpdatePostAsync_NotFound()
        {
            _repo.Setup(r => r.ExistsByUrlSlugAsync(It.IsAny<string>()))
                 .ReturnsAsync(false);
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync((Post)null);
            var result = await _service.UpdatePostAsync(1, new PostSaveDTO());
            Assert.False(result.Ok);
        }

        [Fact]
        public async Task UpdatePostAsync_Valid_UpdatesPost()
        {
            var dto = new PostSaveDTO { UrlSlug = "slug" };
            var post = new Post();
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug))
                 .ReturnsAsync(false);
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(post);
            _mapper.Setup(m => m.Map(dto, post));
            _mapper.Setup(m => m.Map<PostResponse>(post))
                   .Returns(new PostResponse { Ok = true });
            var result = await _service.UpdatePostAsync(1, dto);
            Assert.True(result.Ok);
            _repo.Verify(r => r.UpdateAsync(post), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletePostAsync_NotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync((Post)null);
            var result = await _service.DeletePostAsync(1);
            Assert.False(result.Ok);
        }

        [Fact]
        public async Task DeletePostAsync_Valid_DeletesPost()
        {
            var post = new Post();
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(post);
            _mapper.Setup(m => m.Map<PostResponse>(post))
                   .Returns(new PostResponse { Ok = true });
            var result = await _service.DeletePostAsync(1);
            Assert.True(result.Ok);
            _repo.Verify(r => r.DeleteAsync(post), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SearchAsync_InvalidKeyword_ReturnsEmpty(string keyword)
        {
            var result = await _service.SearchAsync(keyword);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_ValidKeyword_ReturnsResults()
        {
            var posts = new List<Post> { new(), new() };
            _repo.Setup(r => r.SearchAsync("test"))
                 .ReturnsAsync(posts);
            _mapper.Setup(m => m.Map<IEnumerable<PostResponse>>(posts))
                   .Returns(new List<PostResponse> { new(), new() });
            var result = await _service.SearchAsync("test");
            Assert.Equal(2, result.Count());
        }
    }
}
