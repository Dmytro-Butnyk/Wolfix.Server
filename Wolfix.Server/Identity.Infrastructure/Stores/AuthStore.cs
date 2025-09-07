using System.Net;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Projections;
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
        Account? user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result<UserRolesProjection>.Failure($"User with email: {email} not found", HttpStatusCode.NotFound);
        }
        
        bool isPasswordCorrect = await userManager.CheckPasswordAsync(user, password);
        
        if (!isPasswordCorrect)
        {
            return Result<UserRolesProjection>.Failure("Invalid password");
        }
        
        IList<string> userRoles = await userManager.GetRolesAsync(user);

        if (userRoles.Count == 0)
        {
            return Result<UserRolesProjection>.Failure("User does not have any roles", HttpStatusCode.InternalServerError);
        }
        
        UserRolesProjection userRolesProjection = new(user.Id, user.Email!, userRoles);
        return Result<UserRolesProjection>.Success(userRolesProjection);
    }

    public async Task<VoidResult> CheckUserExistsAndHasRole(Guid userId, string role)
    {
        Account? user = await userManager.FindByIdAsync(userId.ToString());
        
        if (user == null)
        {
            return VoidResult.Failure($"User with id: {userId} not found", HttpStatusCode.NotFound);
        }
        
        bool isRoleExists = await roleManager.RoleExistsAsync(role);

        if (!isRoleExists)
        {
            return VoidResult.Failure("Role does not exist", HttpStatusCode.NotFound);
        }
        
        bool hasRole = await userManager.IsInRoleAsync(user, role);

        if (!hasRole)
        {
            return VoidResult.Failure("User does not have required role", HttpStatusCode.Forbidden);
        }
        
        return VoidResult.Success();
    }

    public async Task<Result<Guid>> RegisterAsCustomerAndGetUserIdAsync(string email, string password)
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

        //todo: разобраться с транзакцией
        await using var transaction = await context.Database.BeginTransactionAsync();

        IdentityResult createResult = await userManager.CreateAsync(user, password);
        
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return Result<Guid>.Failure("Failed to create user", HttpStatusCode.InternalServerError);
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, Roles.Customer);

        if (!addRoleResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return Result<Guid>.Failure("Failed to add a role to user", HttpStatusCode.InternalServerError);
        }
        
        await transaction.CommitAsync();
        
        return Result<Guid>.Success(user.Id);
    }
}