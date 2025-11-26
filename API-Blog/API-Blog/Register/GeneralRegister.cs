using API_Blog.Configurations;
using Application.MappingProfiles;
using Asp.Versioning;
using Domain.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;
using System.Reflection;
using System.Text;


namespace API_Blog.Register
{
    public static class GeneralRegister
    {
        private static readonly string PolicyName = "JustBlogApi";
        public static void RegisterGeneralServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigOption(configuration);
            services.VersionApiInjection();
            services.IdentityInjection();
            services.JwtInjection(configuration);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(Assembly.Load("Application"));

            services.AddAutoMapper(cfg => { cfg.AddProfile<AppMappingProfile>(); });
        }

        public static void RegisterGeneralApp(this WebApplication app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();
            app.UseCors(PolicyName);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.MapControllers();
        }

        private static void VersionApiInjection(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        private static void IdentityInjection(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>
                (options =>
                {
                    // Thiết lập về Password
                    options.Password.RequireDigit = false; // Không bắt phải có số
                    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                    options.Password.RequiredLength = 8; // Số ký tự tối thiểu của password
                    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                    // Cấu hình Lockout - khóa user
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                    options.Lockout.AllowedForNewUsers = true;

                    // Cấu hình về User.
                    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true; // Email là duy nhất

                    // Cấu hình đăng nhập.
                    options.SignIn.RequireConfirmedEmail = false; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
                })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();
        }

        private static void JwtInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Config JWT
            var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JwtConfiguration:SecretKey is missing. Add 'JwtConfiguration:SecretKey' to appsettings.json or provide it via environment variables.");
            }

            services.Configure<JwtSetting>(configuration.GetSection("JwtConfiguration"));
            var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userManager =
                                context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                            userManager.GetUserAsync(context.HttpContext.User);
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/realtime"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "You need to login to access." });
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "You do not have access to this function." });
                            return context.Response.WriteAsync(result);
                        }
                    };
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        private static void ConfigOption(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                var withOrigins = configuration.GetSection("WithOrigins").Get<string[]>() ?? Array.Empty<string>();
                options.AddPolicy(PolicyName, policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true).AllowCredentials().WithOrigins(withOrigins);
                });
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = 83886080;
                x.MultipartHeadersLengthLimit = 83886080;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
            });
            services.AddSignalR(
                    hubOptions =>
                    {
                        hubOptions.EnableDetailedErrors = true;
                        hubOptions.MaximumReceiveMessageSize = null;
                    })
                .AddNewtonsoftJsonProtocol(
                    opt =>
                        opt.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }
    }
}
