using Application.Interfaces;
using Application.Services;
using Application.UnitOfWork;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace API_Blog.Register
{
    public static class ApplicationDI
    {
        public static void AddApplicationConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostTagMapService, PostTagMapService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostTagMapRepository, PostTagMapRepository>();
        }
    }
}
