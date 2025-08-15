using Identity.Application.Projections;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Repositories;

public interface IAuthStore
{
    Task<Result<UserRolesProjection>> LogInAndGetUserRolesAsync(string email, string password);
    
    Task<VoidResult> CheckUserExistsAndHasRole(Guid userId, string role);
    
    Task<Result<Guid>> RegisterAndGetUserIdAsync(string email, string password);
}