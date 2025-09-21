using System.Net;
using Identity.Application;
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
    public async Task<Result<UserRolesProjection>> LogInAndGetUserRolesAsync(string email, string password, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        Result<Account> getUserResult = await GetUser(email, password, ct);

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

    public async Task<Result<Guid>> CheckUserExistsAndHasRole(string email, string password, string role, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        Result<Account> getUserResult = await GetUser(email, password, ct);

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

    private async Task<Result<Account>> GetUser(string email, string password, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

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

    public async Task<Result<Guid>> RegisterAccountAsync(string email, string password, string role, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

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

        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        IdentityResult createResult = await userManager.CreateAsync(user, password);
        
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync(ct);
            return Result<Guid>.Failure(createResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, role);

        if (!addRoleResult.Succeeded)
        {
            await transaction.RollbackAsync(ct);
            return Result<Guid>.Failure(addRoleResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        await transaction.CommitAsync(ct);
        
        return Result<Guid>.Success(user.Id);
    }

    public async Task<VoidResult> AddSellerRoleAsync(Guid accountId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Account? account = await userManager.FindByIdAsync(accountId.ToString());
        
        if (account is null)
        {
            return VoidResult.Failure(
                $"Account with id: {accountId} not found",
                HttpStatusCode.NotFound
            );
        }

        bool customerAlreadyHasSellerRole = await userManager.IsInRoleAsync(account, Roles.Seller);

        if (customerAlreadyHasSellerRole)
        {
            return VoidResult.Failure("Customer already has seller role");
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(account, Roles.Seller);
        
        if (!addRoleResult.Succeeded)
        {
            return VoidResult.Failure(addRoleResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> ChangeEmailAsync(Guid accountId, string email, string token, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Account? account = await userManager.FindByIdAsync(accountId.ToString());
        
        if (account is null)
        {
            return VoidResult.Failure(
                "Account not found",
                HttpStatusCode.NotFound
            );
        }

        IdentityResult changeEmailResult = await userManager.ChangeEmailAsync(account, email, token);

        if (!changeEmailResult.Succeeded)
        {
            return VoidResult.Failure(changeEmailResult.GetErrorMessage());
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> ChangePasswordAsync(Guid accountId, string currentPassword, string newPassword, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Account? account = await userManager.FindByIdAsync(accountId.ToString());
        
        if (account is null)
        {
            return VoidResult.Failure(
                $"Account with id: {accountId} not found",
                HttpStatusCode.NotFound
            );
        }

        IdentityResult changePasswordResult = await userManager.ChangePasswordAsync(account, currentPassword, newPassword);

        if (!changePasswordResult.Succeeded)
        {
            return VoidResult.Failure(changePasswordResult.GetErrorMessage());
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> CheckUserCanBeSeller(Guid accountId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        Account? account = await userManager.FindByIdAsync(accountId.ToString());

        if (account is null)
        {
            return VoidResult.Failure(
                $"Account with id: {accountId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        bool userHasSellerRole = await userManager.IsInRoleAsync(account, Roles.Seller);

        if (userHasSellerRole)
        {
            return VoidResult.Failure("User already has seller role");
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> AddAdminRoleAsync(Guid accountId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Account? account = await userManager.FindByIdAsync(accountId.ToString());
        
        if (account is null)
        {
            return VoidResult.Failure(
                $"Account with id: {accountId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        bool customerAlreadyHasAdminRole = await userManager.IsInRoleAsync(account, Roles.Admin);

        if (customerAlreadyHasAdminRole)
        {
            return VoidResult.Failure("Customer already has admin role");
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(account, Roles.Admin);
        
        if (!addRoleResult.Succeeded)
        {
            return VoidResult.Failure(addRoleResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        return VoidResult.Success();
    }
}