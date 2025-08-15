using System.Net;
using Identity.Application.Dto.Responses;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Interfaces.Services;
using Identity.Application.Mapping;
using Identity.Application.Projections;
using Shared.Domain.Models;

namespace Identity.Application.Services;

internal sealed class AuthService(
    IAuthStore authStore,
    IJwtService jwtService) : IAuthService
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

    public async Task<Result<string>> GetTokenByRoleAsync(Guid userId, string email, string role)
    {
        VoidResult checkUserExistsAndHasRoleResult = await authStore.CheckUserExistsAndHasRole(userId, role);

        if (!checkUserExistsAndHasRoleResult.IsSuccess)
        {
            return Result<string>.Failure(checkUserExistsAndHasRoleResult.ErrorMessage!, checkUserExistsAndHasRoleResult.StatusCode);
        }

        string token = jwtService.GenerateToken(userId, email, role);
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsync(string email, string password)
    {
        Result<Guid> registerResult = await authStore.RegisterAndGetUserIdAsync(email, password);

        if (!registerResult.IsSuccess)
        {
            return Result<string>.Failure(registerResult.ErrorMessage!, registerResult.StatusCode);
        }
        
        //todo: решить как роль передавать
        string token = jwtService.GenerateToken(registerResult.Value, email, "user");
        
        return Result<string>.Success(token);
    }
}