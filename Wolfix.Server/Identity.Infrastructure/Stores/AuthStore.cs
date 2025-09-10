using System.Net;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Projections;
using Identity.Infrastructure.Extensions;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Domain.Models;

namespace Identity.Infrastructure.Stores;

//todo: разделить методы по логике
internal sealed class AuthStore(
    IdentityContext context,
    UserManager<Account> userManager,
    RoleManager<Role> roleManager) : IAuthStore
{
    public async Task<Result<UserRolesProjection>> LogInAndGetUserRolesAsync(string email, string password)
    {
        Result<Account> getUserResult = await GetUser(email, password);

        if (!getUserResult.IsSuccess)
        {
            return Result<UserRolesProjection>.Failure(getUserResult.ErrorMessage!, getUserResult.StatusCode);
        }
        
        Account user = getUserResult.Value!;
        
        IList<string> userRoles = await userManager.GetRolesAsync(user);

        if (userRoles.Count == 0)
        {
            return Result<UserRolesProjection>.Failure("User does not have any roles", HttpStatusCode.InternalServerError);
        }
        
        UserRolesProjection userRolesProjection = new(user.Id, user.Email!, userRoles);
        return Result<UserRolesProjection>.Success(userRolesProjection);
    }

    public async Task<Result<Guid>> CheckUserExistsAndHasRole(string email, string password, string role)
    {
        Result<Account> getUserResult = await GetUser(email, password);

        if (!getUserResult.IsSuccess)
        {
            return Result<Guid>.Failure(getUserResult.ErrorMessage!, getUserResult.StatusCode);
        }
        
        Account user = getUserResult.Value!;
        
        bool isRoleExists = await roleManager.RoleExistsAsync(role);

        if (!isRoleExists)
        {
            return Result<Guid>.Failure("Role does not exist", HttpStatusCode.NotFound);
        }
        
        bool hasRole = await userManager.IsInRoleAsync(user, role);

        if (!hasRole)
        {
            return Result<Guid>.Failure("User does not have required role", HttpStatusCode.Forbidden);
        }
        
        return Result<Guid>.Success(user.Id);
    }

    private async Task<Result<Account>> GetUser(string email, string password)
    {
        Account? user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result<Account>.Failure($"User with email: {email} not found", HttpStatusCode.NotFound);
        }
        
        bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
        
        if (!isPasswordCorrect)
        {
            return Result<Account>.Failure("Invalid password");
        }
        
        return Result<Account>.Success(user);
    }

    public async Task<Result<Guid>> RegisterAccountAsync(string email, string password, string role)
    {
        Account? existingUser = await userManager.FindByEmailAsync(email);
        
        if (existingUser != null)
        {
            return Result<Guid>.Failure("This email already taken", HttpStatusCode.Conflict);
        }

        var user = new Account
        {
            Email = email,
            UserName = email
        };

        await using var transaction = await context.Database.BeginTransactionAsync();

        IdentityResult createResult = await userManager.CreateAsync(user, password);
        
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return Result<Guid>.Failure(createResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, role);

        if (!addRoleResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return Result<Guid>.Failure(addRoleResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        await transaction.CommitAsync();
        
        return Result<Guid>.Success(user.Id);
    }
}