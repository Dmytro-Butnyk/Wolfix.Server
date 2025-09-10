using Identity.Application.Projections;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Repositories;

public interface IAuthStore
{
    Task<Result<UserRolesProjection>> LogInAndGetUserRolesAsync(string email, string password);
    
    Task<Result<Guid>> CheckUserExistsAndHasRole(string email, string password, string role);
    
    Task<Result<Guid>> RegisterAccountAsync(string email, string password, string role);
}