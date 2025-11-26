using Application.DTOs.PostTagMap;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;
using AutoMapper;
using Domain.Entities;

namespace Test.Common
{
    public class AutoMapperTestProfile : Profile
    {
        public AutoMapperTestProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<CategorySaveDTO, Category>().ReverseMap();

            CreateMap<Post, PostDTO>();
            CreateMap<Post, PostResponse>();
            CreateMap<PostSaveDTO, Post>().ReverseMap();

            CreateMap<Tag, TagDTO>();
            CreateMap<Tag, TagSaveDTO>().ReverseMap();
            CreateMap<Tag, TagResponse>();

            CreateMap<PostTagMap, PostTagMapDTO>().ReverseMap();
            CreateMap<PostTagMapSaveDTO, PostTagMap>();
            CreateMap<IEnumerable<PostTagMap>, PostTagMapResponse>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.First().PostId))
                .ForMember(dest => dest.TagNames, opt => opt.MapFrom(src => src.Select(x => x.Tag!.Name).ToList()));
        }
    }
}
