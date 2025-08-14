using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<string>> LogInAsync(string email, string password, string role);
    Task<Result<string>> RegisterAsync(string email, string password);
}