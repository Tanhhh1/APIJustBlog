using Infrastructure.Database;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API_Blog.Register
{
    public static class DatabaseDI
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Database");
            services.Configure<DatabaseConfiguration>(section);
            var databaseConfig = section.Get<DatabaseConfiguration>();
            if (databaseConfig == null) 
            {
                throw new Exception("Database configuration not found! Please check 'appsettings.json' file again.");
            }
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(databaseConfig.Main, opt => opt.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName)));
        }
    }
}
