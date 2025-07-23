using Application.Domain.Entities;
using Application.Domain.Shared.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Application.EntityFrameworkCore.Seed;

public static class Seeder
{
    public static async Task SeedAdminUserAndRolesAsync(ApplicationDbContext dbContext)
    {
        // 1. Ensure Role Exists
        var adminRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "Admin");

        if (adminRole == null)
        {
            adminRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
            };

            dbContext.Roles.Add(adminRole);
            await dbContext.SaveChangesAsync();
        }

        // 2. Ensure Admin User Exists
        var adminUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == "admin@system.local");

        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Email = "admin@example.com",
                Name = "Administrator", 
                PhoneNumber = "0000000000",
                PasswordHash = "hashedpassword" /*PasswordHasher.HashPassword("hashedpassword")*/,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            dbContext.Users.Add(adminUser);
            await dbContext.SaveChangesAsync();
        }

        // 3. Assign Role to User if not already
        var hasUserRole = await dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);

        if (!hasUserRole)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
        }

        // 4. Assign all permissions to Admin Role
        var existingPermissions = await dbContext.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.Permission)
            .ToListAsync();

        var allPermissions = PermissionConstants.All;
        var missingPermissions = allPermissions
            .Where(p => !existingPermissions.Contains(p))
            .Select(p => new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = adminRole.Id,
                Permission = p
            });

        dbContext.RolePermissions.AddRange(missingPermissions);

        await dbContext.SaveChangesAsync();
    }
}
