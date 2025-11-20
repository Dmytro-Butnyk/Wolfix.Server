using Identity.Application;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Helpers;

public static class IdentityDbHelper
{
    public static async Task EnsureAllRolesExist(this IdentityContext context, RoleManager<Role> roleManager)
    {
        foreach (var role in Roles.All)
        {
            await EnsureRoleExist(role, roleManager);
        }

        await context.DeleteInvalidRoles(roleManager);
    }

    private static async Task EnsureRoleExist(string role, RoleManager<Role> roleManager)
    {
        var existingRole = await roleManager.FindByNameAsync(role);
        if (existingRole == null)
        {
            var identityResult = await roleManager.CreateAsync(new Role
            {
                Name = role,
                NormalizedName = role.ToUpper(),
                ConcurrencyStamp = Guid.CreateVersion7().ToString()
            });

            if (!identityResult.Succeeded)
            {
                throw new Exception($"Failed to create role: {role}");
            }
        }
    }

    private static async Task DeleteInvalidRoles(this IdentityContext context, RoleManager<Role> roleManager)
    {
        List<Role> otherInvalidRoles = await context.Roles
            .AsNoTracking()
            .Where(role => !Roles.All.Contains(role.Name))
            .ToListAsync();

        foreach (var invalidRole in otherInvalidRoles)
        {
            var deleteResult = await roleManager.DeleteAsync(invalidRole);
            
            if (!deleteResult.Succeeded)
            {
                throw new Exception($"Failed to delete role: {invalidRole.Name}");
            }
        }
    }
}