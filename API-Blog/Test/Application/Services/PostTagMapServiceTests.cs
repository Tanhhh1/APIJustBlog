using Application.DTOs.PostTagMap;
using Application.Services;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interfaces;
using Moq;
using Test.Common;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Application.Services
{
    public class PostTagMapServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IPostRepository> _mockPostRepo;
        private readonly Mock<ITagRepository> _mockTagRepo;
        private readonly Mock<IPostTagMapRepository> _mockPostTagMapRepo;
        private readonly PostTagMapService _service;

        public PostTagMapServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperTestProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _mockUow = new Mock<IUnitOfWork>();
            _mockPostRepo = new Mock<IPostRepository>();
            _mockTagRepo = new Mock<ITagRepository>();
            _mockPostTagMapRepo = new Mock<IPostTagMapRepository>();

            _mockUow.Setup(u => u.PostRepository).Returns(_mockPostRepo.Object);
            _mockUow.Setup(u => u.TagRepository).Returns(_mockTagRepo.Object);
            _mockUow.Setup(u => u.PostTagMapRepository).Returns(_mockPostTagMapRepo.Object);
            _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            _service = new PostTagMapService(_mockUow.Object, _mapper);
        }

        [Fact]
        public async Task CreateLinkAsync_ReturnCreatedResponse()
        {
            var dto = TestDataFactory.CreatePostTagMapSaveDTO();

            _mockPostRepo.Setup(r => r.GetByIdAsync(dto.PostId))
                         .ReturnsAsync(TestDataFactory.CreatePost(dto.PostId));

            _mockTagRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestDataFactory.CreateTag(1));
            _mockTagRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(TestDataFactory.CreateTag(2));

            var addedLinks = new List<PostTagMap>();
            _mockPostTagMapRepo.Setup(r => r.GetByPostIdAsync(dto.PostId))
                               .ReturnsAsync(() => addedLinks);

            _mockPostTagMapRepo.Setup(r => r.AddAsync(It.IsAny<PostTagMap>()))
                               .Callback<PostTagMap>(ptm =>
                               {
                                   ptm.Tag = TestDataFactory.CreateTag(ptm.TagId);
                                   ptm.Post = TestDataFactory.CreatePost(ptm.PostId);
                                   addedLinks.Add(ptm);
                               })
                               .Returns(Task.CompletedTask);

            var result = await _service.CreateLinkAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(2, addedLinks.Count);       
            Assert.Equal(dto.PostId, result.PostId); 
            _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateLinkAsync_PostNotFound()
        {
            var dto = TestDataFactory.CreatePostTagMapSaveDTO();
            _mockPostRepo.Setup(r => r.GetByIdAsync(dto.PostId)).ReturnsAsync((Post)null);
            var result = await _service.CreateLinkAsync(dto);
            Assert.Null(result);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateLinkAsync_TagsNotFound()
        {
            var dto = TestDataFactory.CreatePostTagMapSaveDTO();
            _mockPostRepo.Setup(r => r.GetByIdAsync(dto.PostId)).ReturnsAsync(TestDataFactory.CreatePost(dto.PostId));
            _mockTagRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestDataFactory.CreateTag(1));
            _mockTagRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Tag)null);
            _mockPostTagMapRepo.Setup(r => r.GetByPostIdAsync(dto.PostId))
                               .ReturnsAsync(new List<PostTagMap> { TestDataFactory.CreatePostTagMap(dto.PostId, 1) });
            var result = await _service.CreateLinkAsync(dto);
            Assert.NotNull(result);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateLinkAsync_ExceptionPropagates()
        {
            var dto = TestDataFactory.CreatePostTagMapSaveDTO();
            _mockPostRepo.Setup(r => r.GetByIdAsync(dto.PostId)).ReturnsAsync(TestDataFactory.CreatePost(dto.PostId));
            _mockTagRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(TestDataFactory.CreateTag(1));
            _mockPostTagMapRepo.Setup(r => r.GetByPostIdAsync(dto.PostId)).ReturnsAsync(new List<PostTagMap>());
            _mockPostTagMapRepo.Setup(r => r.AddAsync(It.IsAny<PostTagMap>())).ThrowsAsync(new Exception("Add failed"));

            await Assert.ThrowsAsync<Exception>(() => _service.CreateLinkAsync(dto));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task GetLinkByIdAsync_PostNotFound()
        {
            _mockPostRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post)null);
            var result = await _service.GetLinkByIdAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLinkByIdAsync_NoLinks()
        {
            int postId = 1;
            _mockPostRepo.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(TestDataFactory.CreatePost(postId));
            _mockPostTagMapRepo.Setup(r => r.GetByPostIdAsync(postId)).ReturnsAsync(new List<PostTagMap>());

            var result = await _service.GetLinkByIdAsync(postId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteLinkAsync_ReturnsUpdated()
        {
            int postId = 1, tagId = 2;
            _mockPostRepo.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(TestDataFactory.CreatePost(postId));
            var existingLinks = new List<PostTagMap>
            {
                TestDataFactory.CreatePostTagMap(postId, 1),
                TestDataFactory.CreatePostTagMap(postId, tagId)
            };

            _mockPostTagMapRepo.SetupSequence(r => r.GetByPostIdAsync(postId))
                               .ReturnsAsync(existingLinks)
                               .ReturnsAsync(existingLinks.Where(x => x.TagId != tagId).ToList());

            var deletedLinks = new List<PostTagMap>();
            _mockPostTagMapRepo.Setup(r => r.DeleteAsync(It.IsAny<PostTagMap>()))
                               .Callback<PostTagMap>(link => deletedLinks.Add(link));

            var result = await _service.DeleteLinkAsync(postId, tagId);
            Assert.NotNull(result);
            Assert.Single(deletedLinks);
            Assert.Equal(tagId, deletedLinks.First().TagId);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteLinkAsync_PostNotFound()
        {
            _mockPostRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post)null);
            var result = await _service.DeleteLinkAsync(1, 1);
            Assert.Null(result);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteLinkAsync_LinkNotFound()
        {
            int postId = 1, tagId = 3;
            _mockPostRepo.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(TestDataFactory.CreatePost(postId));
            _mockPostTagMapRepo.Setup(r => r.GetByPostIdAsync(postId))
                               .ReturnsAsync(new List<PostTagMap>
                               {
                                   TestDataFactory.CreatePostTagMap(postId, 1),
                                   TestDataFactory.CreatePostTagMap(postId, 2)
                               });

            var result = await _service.DeleteLinkAsync(postId, tagId);

            Assert.Null(result);
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteLinkAsync_ExceptionPropagates()
        {
            int postId = 1, tagId = 2;
            _mockPostRepo.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(TestDataFactory.CreatePost(postId));
            var existingLinks = new List<PostTagMap>
            {
                TestDataFactory.CreatePostTagMap(postId, 1),
                TestDataFactory.CreatePostTagMap(postId, tagId)
            };
            _mockPostTagMapRepo.Setup(r => r.GetByPostIdAsync(postId)).ReturnsAsync(existingLinks);
            _mockPostTagMapRepo.Setup(r => r.DeleteAsync(It.IsAny<PostTagMap>())).ThrowsAsync(new Exception("Delete failed"));
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteLinkAsync(postId, tagId));
            _mockUow.Verify(u => u.CompleteAsync(), Times.Never);
        }
    }
}
