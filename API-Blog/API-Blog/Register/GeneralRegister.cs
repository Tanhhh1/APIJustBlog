using API_Blog.Configurations;
using Application.MappingProfiles;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;


namespace API_Blog.Register
{
    public static class GeneralRegister
    {
        public static void RegisterGeneralServices(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddFluentValidationAutoValidation(); 
            services.AddValidatorsFromAssembly(Assembly.Load("Application"));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(cfg => { cfg.AddProfile<AppMappingProfile>(); });
        }

        public static void RegisterGeneralApp(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapControllers();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }


    }
}
