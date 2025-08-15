using System.Net;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Interfaces.Services;
using Shared.Domain.Models;

namespace Identity.Application.Services;

internal sealed class AuthService(
    IAuthStore authStore,
    IJwtService jwtService) : IAuthService
{
    public async Task<Result<string>> LogInAsync(string email, string password, string role)
    {
        Result<Guid> logInResult = await authStore.LogInAndGetUserIdAsync(email, password, role);

        if (!logInResult.IsSuccess)
        {
            return Result<string>.Failure(logInResult.ErrorMessage!, logInResult.StatusCode);
        }
        
        string token = jwtService.GenerateToken(logInResult.Value, email, role);
        
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