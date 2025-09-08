using Identity.Application.Projections;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Repositories;

public interface IAuthStore
{
    Task<Result<UserRolesProjection>> LogInAndGetUserRolesAsync(string email, string password);
    
    Task<Result<Guid>> CheckUserExistsAndHasRole(string email, string role);
    
    Task<Result<Guid>> RegisterAsCustomerAndGetUserIdAsync(string email, string password);
}