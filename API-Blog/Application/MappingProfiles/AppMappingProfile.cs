using Application.DTOs.PostTagMap;
using Application.Models.Category.DTO;
using Application.Models.Category.Response;
using Application.Models.Post.DTO;
using Application.Models.Post.Response;
using Application.Models.Tag.DTO;
using Application.Models.Tag.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<Category, CategoryDTO>();
        CreateMap<Category, CategorySaveDTO>().ReverseMap();
        CreateMap<Category, CategoryResponse>();

        CreateMap<Tag, TagDTO>();
        CreateMap<Tag, TagSaveDTO>().ReverseMap();
        CreateMap<Tag, TagResponse>();

        CreateMap<Post, PostDTO>();
        CreateMap<Post, PostSaveDTO>().ReverseMap();
        CreateMap<Post, PostResponse>();

        CreateMap<PostTagMap, PostTagMapDTO>().ReverseMap();
        CreateMap<PostTagMapSaveDTO, PostTagMap>();
        CreateMap<IEnumerable<PostTagMap>, PostTagMapResponse>()
             .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.FirstOrDefault()!.PostId))
             .ForMember(dest => dest.TagNames, opt => opt.MapFrom(src => src.Select(x => x.Tag!.Name).ToList()));

    }
}