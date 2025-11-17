using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace API_Blog.Register
{
    public static class SwaggerRegister
    {
        public static void RegisterSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Put **_ONLY_** your JWT Bearer token on text box below!",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                opt.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    jwtSecurityScheme,Array.Empty<string>()
                }
            });

                //Add detail xml 
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlFilePath);
            });
        }


        public static void RegisterSwaggerApp(this WebApplication app, WebApplicationBuilder builder)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swagger, req) =>
                {
                    swagger.Servers = new List<OpenApiServer>() { new OpenApiServer() { Url = builder.Configuration["ServerSetting:CurrentDomain"] } };
                });
            });

            app.UseSwaggerUI(options =>
            {
                foreach (var desc in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"{builder.Configuration["ServerSetting:Name"]} V{desc.ApiVersion}");
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                }
            });
        }
    }
}
