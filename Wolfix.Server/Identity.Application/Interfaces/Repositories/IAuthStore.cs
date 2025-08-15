using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Repositories;

public interface IAuthStore
{
    Task<Result<Guid>> LogInAndGetUserIdAsync(string email, string password, string role);
    
    Task<Result<Guid>> RegisterAndGetUserIdAsync(string email, string password);
}