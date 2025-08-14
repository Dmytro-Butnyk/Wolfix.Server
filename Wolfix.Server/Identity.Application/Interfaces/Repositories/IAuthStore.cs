namespace Identity.Application.Interfaces.Repositories;

public interface IAuthStore
{
    Task<Guid?> LogInAndGetUserIdAsync(string email, string password, string role);
    
    Task<Guid?> RegisterAndGetUserIdAsync(string email, string password);
}