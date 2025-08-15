using System.Net;
using Identity.Application.Interfaces.Repositories;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Domain.Models;

namespace Identity.Infrastructure.Stores;

internal sealed class AuthStore(UserManager<Account> userManager) : IAuthStore
{
    public async Task<Result<Guid>> LogInAndGetUserIdAsync(string email, string password, string role)
    {
        Account? user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return Result<Guid>.Failure($"User with email: {email} not found", HttpStatusCode.NotFound);
        }
        
        bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
        
        if (!isPasswordCorrect)
        {
            return Result<Guid>.Failure("Invalid password");
        }
        
        bool hasRole = await userManager.IsInRoleAsync(user, role);

        if (!hasRole)
        {
            return Result<Guid>.Failure("User does not have required role", HttpStatusCode.Forbidden);
        }
        
        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<Guid>> RegisterAndGetUserIdAsync(string email, string password)
    {
        Account? existingUser = await userManager.FindByEmailAsync(email);
        
        if (existingUser != null)
        {
            return Result<Guid>.Failure("User already exists", HttpStatusCode.Conflict);
        }

        var user = new Account
        {
            Email = email,
            UserName = email
        };

        IdentityResult createResult = await userManager.CreateAsync(user, password);
        
        if (!createResult.Succeeded)
        {
            return Result<Guid>.Failure("Failed to create user", HttpStatusCode.InternalServerError);
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, "User");

        if (!addRoleResult.Succeeded)
        {
            return Result<Guid>.Failure("Failed to add a role to user", HttpStatusCode.InternalServerError);
        }
        
        return Result<Guid>.Success(user.Id);
    }
}