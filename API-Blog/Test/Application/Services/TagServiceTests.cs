using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Application.Models.Post.Response;
using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Moq;


namespace Test.Application.Services
{
    public class TagServiceTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<ITagRepository> _repo;
        private readonly TagService _service;

        public TagServiceTests()
        {
            _mapper = new Mock<IMapper>();
            _repo = new Mock<ITagRepository>();
            _uow = new Mock<IUnitOfWork>();

            _uow.Setup(u => u.TagRepository).Returns(_repo.Object);
            _uow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _service = new TagService(_uow.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetAllTagAsync_Success()
        {
            var tags = new List<Tag>
            {
                new() { Name = "Tag1" },
                new() { Name = "Tag2" }
            };
            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(tags);
            _mapper.Setup(m => m.Map<IEnumerable<TagDTO>>(It.IsAny<IEnumerable<Tag>>()))
                   .Returns(new List<TagDTO> { new(), new() });
            var result = await _service.GetAllTagAsync();
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllTagAsync_Exception_ThrowsBadRequest()
        {
            _repo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB error"));
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.GetAllTagAsync()
            );
        }

        [Fact]
        public async Task GetByTagIdAsync_Found()
        {
            var tag = new Tag();
            var dto = new TagDTO();
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);
            _mapper.Setup(m => m.Map<TagDTO>(tag)).Returns(dto);
            var result = await _service.GetByTagIdAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByTagIdAsync_NotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Tag?)null);
            var result = await _service.GetByTagIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTagAsync_UrlSlugExists_Throws()
        {
            var dto = new TagSaveDTO { UrlSlug = "slug" };
            _repo.Setup(r => r.ExistsByUrlSlugAsync("slug")).ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.CreateTagAsync(dto));
        }

        [Fact]
        public async Task CreateTagAsync_Valid_CreatesTag()
        {
            var dto = new TagSaveDTO() { UrlSlug = "slug" };
            var tag = new Tag();
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug)).ReturnsAsync(false);
            _mapper.Setup(m => m.Map<Tag>(dto)).Returns(tag);
            _mapper.Setup(m => m.Map<TagResponse>(tag)).Returns(new TagResponse { Ok = true });
            var result = await _service.CreateTagAsync(dto);
            Assert.True(result.Ok);
            _repo.Verify(r => r.AddAsync(tag), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTagAsync_UrlSlugExists_Throws()
        {
            var dto = new TagSaveDTO { UrlSlug = "slug" };
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug))
                 .ReturnsAsync(true);
            await Assert.ThrowsAsync<BadRequestException>(
                () => _service.UpdateTagAsync(1, dto));
        }

        [Fact]
        public async Task UpdateTagAsync_NotFound()
        {
            _repo.Setup(r => r.ExistsByUrlSlugAsync(It.IsAny<string>()))
                 .ReturnsAsync(false);
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync((Tag)null);
            var result = await _service.UpdateTagAsync(1, new TagSaveDTO());
            Assert.False(result.Ok);
        }

        [Fact]
        public async Task UpdateTagAsync_Valid_UpdatesPost()
        {
            var dto = new TagSaveDTO { UrlSlug = "slug" };
            var tag = new Tag();
            _repo.Setup(r => r.ExistsByUrlSlugAsync(dto.UrlSlug))
                 .ReturnsAsync(false);
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(tag);
            _mapper.Setup(m => m.Map(dto, tag));
            _mapper.Setup(m => m.Map<TagResponse>(tag))
                   .Returns(new TagResponse { Ok = true });
            var result = await _service.UpdateTagAsync(1, dto);
            Assert.True(result.Ok);
            _repo.Verify(r => r.UpdateAsync(tag), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletePostAsync_NotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync((Tag)null);
            var result = await _service.DeleteTagAsync(1);
            Assert.False(result.Ok);
        }

        [Fact]
        public async Task DeletePostAsync_Valid_DeletesPost()
        {
            var tag = new Tag();
            _repo.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(tag);
            _mapper.Setup(m => m.Map<TagResponse>(tag))
                   .Returns(new TagResponse { Ok = true });
            var result = await _service.DeleteTagAsync(1);
            Assert.True(result.Ok);
            _repo.Verify(r => r.DeleteAsync(tag), Times.Once);
            _uow.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}
