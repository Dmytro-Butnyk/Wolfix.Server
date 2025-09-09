using System.Net;
using Identity.Application.Dto.Responses;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Interfaces.Services;
using Identity.Application.Mapping;
using Identity.Application.Projections;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.Services;

internal sealed class AuthService(
    IAuthStore authStore,
    IJwtService jwtService,
    IEventBus eventBus) : IAuthService
{
    public async Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(string email, string password)
    {
        Result<UserRolesProjection> getUserRolesResult = await authStore.LogInAndGetUserRolesAsync(email, password);

        if (!getUserRolesResult.IsSuccess)
        {
            return Result<UserRolesDto>.Failure(getUserRolesResult.ErrorMessage!, getUserRolesResult.StatusCode);
        }

        UserRolesDto dto = getUserRolesResult.Value!.ToDto();
        return Result<UserRolesDto>.Success(dto);
    }

    public async Task<Result<string>> GetTokenByRoleAsync(string email, string password, string role)
    {
        Result<Guid> checkUserExistsAndHasRoleResult = await authStore.CheckUserExistsAndHasRole(email, password, role);

        if (!checkUserExistsAndHasRoleResult.IsSuccess)
        {
            return Result<string>.Failure(checkUserExistsAndHasRoleResult.ErrorMessage!, checkUserExistsAndHasRoleResult.StatusCode);
        }

        string token = jwtService.GenerateToken(checkUserExistsAndHasRoleResult.Value, email, role);
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsCustomerAsync(string email, string password, CancellationToken ct)
    {
        Result<Guid> registerResult = await authStore.RegisterAsCustomerAndGetUserIdAsync(email, password, Roles.Customer);

        if (!registerResult.IsSuccess)
        {
            return Result<string>.Failure(registerResult.ErrorMessage!, registerResult.StatusCode);
        }
        
        Guid createdUserId = registerResult.Value;
        
        string token = jwtService.GenerateToken(createdUserId, email, Roles.Customer);

        VoidResult publishResult = await eventBus.PublishAsync(new CustomerAccountCreated
        {
            AccountId = createdUserId
        }, ct);

        if (!publishResult.IsSuccess)
        {
            return Result<string>.Failure(publishResult.ErrorMessage!, publishResult.StatusCode);
        }
        
        return Result<string>.Success(token);
    }
}