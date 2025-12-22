using Application.DTOs.PostTagMap;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Test.Application.Services
{
    public class PostTagMapServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<IMapper> _mapper;

        private readonly Mock<IPostRepository> _postRepo;
        private readonly Mock<ITagRepository> _tagRepo;
        private readonly Mock<IPostTagMapRepository> _mapRepo;

        private readonly PostTagMapService _service;

        public PostTagMapServiceTests()
        {
            _uow = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();

            _postRepo = new Mock<IPostRepository>();
            _tagRepo = new Mock<ITagRepository>();
            _mapRepo = new Mock<IPostTagMapRepository>();

            _uow.Setup(u => u.PostRepository).Returns(_postRepo.Object);
            _uow.Setup(u => u.TagRepository).Returns(_tagRepo.Object);
            _uow.Setup(u => u.PostTagMapRepository).Returns(_mapRepo.Object);
            _uow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _service = new PostTagMapService(_uow.Object, _mapper.Object);
        }

        [Fact]
        public async Task CreateLinkAsync_Success()
        {
            var dto = new PostTagMapSaveDTO
            {
                PostId = 1,
                TagIds = new List<int> { 1 }
            };
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Post { Id = 1 });
            _tagRepo.Setup(r => r.GetByIdAsync(1))
                    .ReturnsAsync(new Tag { Id = 1 });
            _mapRepo.Setup(r => r.GetByPostIdAsync(1))
                    .ReturnsAsync(new List<PostTagMap>());
            _mapper.Setup(m => m.Map<PostTagMapResponse>(It.IsAny<IEnumerable<PostTagMap>>()))
                   .Returns(new PostTagMapResponse());
            var result = await _service.CreateLinkAsync(dto);
            Assert.NotNull(result);
            _mapRepo.Verify(r => r.AddAsync(It.IsAny<PostTagMap>()), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateLinkAsync_NotFound()
        {
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync((Post)null);
            var result = await _service.CreateLinkAsync(
                new PostTagMapSaveDTO { PostId = 1 });
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLinkAsync_Exception_ThrowsBadRequest()
        {
            _postRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                     .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.CreateLinkAsync(new PostTagMapSaveDTO { PostId = 1 }));
        }

        [Fact]
        public async Task GetLinkByIdAsync_Success()
        {
            var maps = new List<PostTagMap>
            {
                new() { PostId = 1, TagId = 1 }
            };
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Post());
            _mapRepo.Setup(r => r.GetByPostIdAsync(1))
                    .ReturnsAsync(maps);
            _mapper.Setup(m => m.Map<PostTagMapResponse>(maps))
                   .Returns(new PostTagMapResponse());
            var result = await _service.GetLinkByIdAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetLinkByIdAsync_NoLinks()
        {
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Post());
            _mapRepo.Setup(r => r.GetByPostIdAsync(1))
                    .ReturnsAsync(new List<PostTagMap>());
            var result = await _service.GetLinkByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLinkByIdAsync_Exception_ThrowsBadRequest()
        {
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetLinkByIdAsync(1));
        }

        [Fact]
        public async Task DeleteLinkAsync_Success()
        {
            var maps = new List<PostTagMap>{ new() { PostId = 1, TagId = 1 } };
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Post());
            _mapRepo.Setup(r => r.GetByPostIdAsync(1))
                    .ReturnsAsync(maps);
            _mapper.Setup(m => m.Map<PostTagMapResponse>(It.IsAny<IEnumerable<PostTagMap>>()))
                   .Returns(new PostTagMapResponse());
            var result = await _service.DeleteLinkAsync(1, 1);
            Assert.NotNull(result);
            _mapRepo.Verify(r => r.DeleteAsync(It.IsAny<PostTagMap>()), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteLinkAsync_NotFound()
        {
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync(new Post());
            _mapRepo.Setup(r => r.GetByPostIdAsync(1))
                    .ReturnsAsync(new List<PostTagMap>());
            var result = await _service.DeleteLinkAsync(1, 99);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteLinkAsync_Exception_ThrowsBadRequest()
        {
            _postRepo.Setup(r => r.GetByIdAsync(1))
                     .ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.DeleteLinkAsync(1, 1));
        }
    }
}
