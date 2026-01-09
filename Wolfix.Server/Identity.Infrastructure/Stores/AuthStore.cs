using System.Net;
using Identity.Application;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Projections;
using Identity.Infrastructure.Enums;
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

        Result<Account> getUserResult = await GetUserAsync(email, password, ct);

        if (!getUserResult.IsSuccess)
        {
            return Result<UserRolesProjection>.Failure(getUserResult.ErrorMessage!, getUserResult.StatusCode);
        }
        
        Account user = getUserResult.Value!;

        if (user.AuthProvider != AccountAuthProvider.Custom)
        {
            return Result<UserRolesProjection>.Failure("User is not registered with custom auth provider", HttpStatusCode.Forbidden);
        }
        
        IList<string> userRoles = await userManager.GetRolesAsync(user);

        if (userRoles.Count == 0)
        {
            return Result<UserRolesProjection>.Failure("User does not have any roles", HttpStatusCode.InternalServerError);
        }
        
        UserRolesProjection userRolesProjection = new(user.Id, user.Email!, userRoles);
        return Result<UserRolesProjection>.Success(userRolesProjection);
    }

    public async Task<Result<Guid>> CheckUserExistsAsync(string email, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        Account? user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result<Guid>.Failure($"User with email: {email} not found", HttpStatusCode.NotFound);
        }
        
        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result<Guid>> CheckUserExistsAndHasRoleAsync(string email, string password, string role,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        Result<Account> getUserResult = await GetUserAsync(email, password, ct);

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

    private async Task<Result<Account>> GetUserAsync(string email, string password, CancellationToken ct)
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

    public async Task<Result<Guid>> RegisterAccountAsync(string email, string password, string role, CancellationToken ct, string authProvider = "Custom")
    {
        ct.ThrowIfCancellationRequested();

        Account? existingUser = await userManager.FindByEmailAsync(email);
        
        if (existingUser != null)
        {
            return Result<Guid>.Failure("This email already taken", HttpStatusCode.Conflict);
        }

        if (!Enum.TryParse<AccountAuthProvider>(authProvider, out var authProviderEnum)) 
        {
            return Result<Guid>.Failure($"Invalid or unknown auth provider: {authProvider}");
        }

        var user = new Account
        {
            Email = email,
            UserName = email,
            AuthProvider = authProviderEnum
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

    public async Task<VoidResult> CheckUserCanBeSellerAsync(Guid accountId, CancellationToken ct)
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

    public async Task<VoidResult> AddRoleAsync(Guid accountId, string role, CancellationToken ct)
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

        bool customerAlreadyHasThisRole = await userManager.IsInRoleAsync(account, role);

        if (customerAlreadyHasThisRole)
        {
            return VoidResult.Failure($"Customer already has {role} role");
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(account, role);
        
        if (!addRoleResult.Succeeded)
        {
            return VoidResult.Failure(addRoleResult.GetErrorMessage(), HttpStatusCode.InternalServerError);
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> RemoveSellerRoleAsync(Guid accountId, CancellationToken ct)
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
        
        bool customerHasSellerRole = await userManager.IsInRoleAsync(account, Roles.Seller);

        if (customerHasSellerRole is false)
        {
            return VoidResult.Failure("Customer doesnt have Seller role");
        }
        
        IdentityResult removeRoleResult = await userManager.RemoveFromRoleAsync(account, Roles.Seller);

        if (!removeRoleResult.Succeeded)
        {
            return VoidResult.Failure(
                removeRoleResult.GetErrorMessage(),
                HttpStatusCode.InternalServerError
            );
        }
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> RemoveRoleOrWholeAccountAsync(Guid accountId, string role, CancellationToken ct)
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

        bool accountHasThisRole = await userManager.IsInRoleAsync(account, role);

        if (accountHasThisRole is false)
        {
            return VoidResult.Failure($"Account doesnt have {role} role");
        }

        bool accountHaveOnlyThisRole = (await userManager.GetRolesAsync(account)).Count == 1;

        if (accountHaveOnlyThisRole)
        {
            IdentityResult deleteAccountResult = await userManager.DeleteAsync(account);

            if (!deleteAccountResult.Succeeded)
            {
                return VoidResult.Failure(
                    deleteAccountResult.GetErrorMessage(),
                    HttpStatusCode.InternalServerError
                );
            }
            
            return VoidResult.Success();
        }

        IdentityResult removeFromRoleResult = await userManager.RemoveFromRoleAsync(account, role);

        if (!removeFromRoleResult.Succeeded)
        {
            return VoidResult.Failure(
                removeFromRoleResult.GetErrorMessage(),
                HttpStatusCode.InternalServerError
            );
        }
        
        return VoidResult.Success();
    }
}