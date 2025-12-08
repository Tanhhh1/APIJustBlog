using Domain.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Shared.Constants;

namespace API.Register;

public static class ResourceRegister
{
    public static async Task RegisterSeedDatabase(this WebApplication app)
    {
        // Migrate Seed Db
        using var scope = app.Services.CreateScope();
        await MigrateAsync(scope.ServiceProvider);
    }

    private static async Task MigrateAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var context = services.GetRequiredService<DatabaseContext>();
        await SeedDatabaseAsync(roleManager, userManager, context);
    }

    private static async Task SeedDatabaseAsync(RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager, DatabaseContext context)
    {
        if (!roleManager.Roles.Any())
        {
            var adminRole = new AppRole()
            {
                Id = Guid.NewGuid(),
                Name = UserRoleConst.Admin,
                NormalizedName = UserRoleConst.Admin.ToUpper(),
                Description = UserRoleConst.Admin
            };
            await roleManager.CreateAsync(adminRole);

            var memberRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = UserRoleConst.Member,
                NormalizedName = UserRoleConst.Member.ToUpper(),
                Description = UserRoleConst.Member
            };
            await roleManager.CreateAsync(memberRole);

            var guestRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = UserRoleConst.Guest,
                NormalizedName = UserRoleConst.Guest.ToUpper(),
                Description = UserRoleConst.Guest
            };
            await roleManager.CreateAsync(guestRole);
        }
    }
}