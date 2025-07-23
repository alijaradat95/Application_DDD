using Application.Domain.Entities;
using Application.Domain.Shared.Permissions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Application.EntityFrameworkCore.Seed
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole == null)
                return;

            var existingPermissions = await context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.Permission)
                .ToListAsync();

            var allPermissions = new[]
            {
                PermissionConstants.Users_Create,
                PermissionConstants.Users_Edit,
                PermissionConstants.Users_Delete,
                PermissionConstants.Users_View,

                PermissionConstants.Roles_Create,
                PermissionConstants.Roles_Edit,
                PermissionConstants.Roles_Delete,
                PermissionConstants.Roles_View
            };

            var newPermissions = allPermissions
                .Except(existingPermissions)
                .Select(p => new RolePermission
                {
                    RoleId = adminRole.Id,
                    Permission = p
                });

            if (newPermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(newPermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}
