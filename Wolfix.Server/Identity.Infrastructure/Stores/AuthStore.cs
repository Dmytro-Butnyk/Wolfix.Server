using Identity.Application.Interfaces.Repositories;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Stores;

internal sealed class AuthStore(
    UserManager<Account> userManager,
    RoleManager<Role> roleManager) : IAuthStore
{
    public async Task<Guid?> LogInAndGetUserIdAsync(string email, string password, string role)
    {
        Account? user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return null;
        }
        
        bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
        
        if (!isPasswordCorrect)
        {
            return null;
        }
        
        bool hasRole = await userManager.IsInRoleAsync(user, role);

        if (!hasRole)
        {
            return null;
        }
        
        return user.Id;
    }

    public async Task<Guid?> RegisterAndGetUserIdAsync(string email, string password)
    {
        Account? existingUser = await userManager.FindByEmailAsync(email);
        
        if (existingUser != null)
        {
            return null;
        }

        var user = new Account
        {
            Email = email,
            UserName = email
        };

        IdentityResult createResult = await userManager.CreateAsync(user, password);
        
        if (!createResult.Succeeded)
        {
            return null;
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, "User");

        if (!addRoleResult.Succeeded)
        {
            return null;
        }
        
        return user.Id;
    }
}