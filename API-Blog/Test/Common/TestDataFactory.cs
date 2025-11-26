using Application.DTOs.PostTagMap;
using Application.Models.Category.DTO;
using Application.Models.Post.DTO;
using Application.Models.Tag.DTO;
using Domain.Entities;

namespace Test.Common
{
    public static class TestDataFactory
    {
        public static Category CreateCategory(
            int id = 1, string name = null, 
            string urlSlug = null, 
            string description = null)
        {
            return new Category
            {
                Id = id,
                Name = name ?? "Category default",
                UrlSlug = urlSlug ?? "category-default",
                Description = description ?? "Default Description"
            };
        }

        public static CategorySaveDTO CreateCategorySaveDTO(
            string name = null, 
            string urlSlug = null, 
            string description = null)
        {
            return new CategorySaveDTO
            {
                Name = name ?? "Category default",
                UrlSlug = urlSlug ?? "category-default",
                Description = description ?? "Default Description"
            };
        }

        public static Post CreatePost(
           int id = 1,
           string title = null,
           int categoryId = 1,
           string shortDescription = null,
           string description = null,
           string meta = null,
           string urlSlug = null,
           bool published = true)
        {
            return new Post
            {
                Id = id,
                Title = title ?? "Post Title default",
                CategoryId = categoryId,
                ShortDescription = shortDescription ?? "Short description default",
                Description = description ?? "Full description default",
                Meta = meta ?? "Meta information default",
                UrlSlug = urlSlug ?? "post-default",
                Published = published
            };
        }

        public static PostSaveDTO CreatePostSaveDTO(
            string title = null,
            int categoryId = 1,
            string shortDescription = null,
            string description = null,
            string meta = null,
            string urlSlug = null,
            bool published = true)
        {
            return new PostSaveDTO
            {
                Title = title ?? "Post Title default",
                CategoryId = categoryId,
                ShortDescription = shortDescription ?? "Short description default",
                Description = description ?? "Full description default",
                Meta = meta ?? "Meta information default",
                UrlSlug = urlSlug ?? "post-default",
                Published = published
            };
        }

        public static Tag CreateTag(
            int id = 1, 
            string name = null, 
            string urlSlug = null, 
            string description = null)
        {
            return new Tag
            {
                Id = id,
                Name = name ?? $"Tag default",
                UrlSlug = urlSlug ?? $"tag-default",
                Description = description ?? $"Description default"
            };
        }

        public static TagSaveDTO CreateTagSaveDTO(
           string name = null,
           string urlSlug = null,
           string description = null)
        {
            return new TagSaveDTO
            {
                Name = name ?? $"Tag default",
                UrlSlug = urlSlug ?? $"tag-default",
                Description = description ?? $"Description default"
            };
        }

        public static PostTagMap CreatePostTagMap(int postId = 1, int tagId = 1)
        {
            return new PostTagMap
            {
                PostId = postId,
                TagId = tagId,
                Tag = CreateTag(tagId),
                Post = CreatePost(postId)
            };
        }

        public static PostTagMapSaveDTO CreatePostTagMapSaveDTO(
            int postId = 1,
            List<int> tagIds = null)
        {
            return new PostTagMapSaveDTO
            {
                PostId = postId,
                TagIds = tagIds ?? new List<int> { 1, 2 }
            };
        }
    }
}
