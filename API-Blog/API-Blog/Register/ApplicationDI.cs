using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Models;
using Application.Services;
using Application.Services.Auth;
using Application.UnitOfWork;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Shared.ClaimService;


namespace API_Blog.Register
{
    public static class ApplicationDI
    {
        public static void AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<ClaimService>();

            services.Configure<EmailSetting>(configuration.GetSection("EmailConfiguration"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostTagMapService, PostTagMapService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostTagMapRepository, PostTagMapRepository>();
        }
    }
}
