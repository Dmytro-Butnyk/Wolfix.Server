using Identity.Application.Dto.Responses;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(string email, string password);
    Task<Result<string>> GetTokenByRoleAsync(Guid userId, string email, string role);
    Task<Result<string>> RegisterAsCustomerAsync(string email, string password, CancellationToken ct);
}