using Identity.Application.Projections;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Repositories;

public interface IAuthStore
{
    Task<Result<UserRolesProjection>> LogInAndGetUserRolesAsync(string email, string password, CancellationToken ct);
    
    Task<Result<Guid>> CheckUserExistsAsync(string email, CancellationToken ct);
    
    Task<Result<Guid>> CheckUserExistsAndHasRoleAsync(string email, string password, string role, CancellationToken ct);
    
    Task<Result<Guid>> RegisterAccountAsync(string email, string password, string role, CancellationToken ct, string? authProvider = "Custom");
    
    Task<VoidResult> AddSellerRoleAsync(Guid accountId, CancellationToken ct);
    
    Task<VoidResult> ChangeEmailAsync(Guid accountId, string email, string token, CancellationToken ct);
    
    Task<VoidResult> ChangePasswordAsync(Guid accountId, string currentPassword, string newPassword, CancellationToken ct);
    
    Task<VoidResult> CheckUserCanBeSellerAsync(Guid accountId, CancellationToken ct);
    
    Task<VoidResult> AddRoleAsync(Guid accountId, string role, CancellationToken ct);
    
    Task<VoidResult> RemoveSellerRoleAsync(Guid accountId, CancellationToken ct);
}