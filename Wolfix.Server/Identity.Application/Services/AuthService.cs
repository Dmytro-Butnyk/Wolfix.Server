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
        Guid? userId = await authStore.LogInAndGetUserIdAsync(email, password, role);
        
        if (userId == null)
        {
            return Result<string>.Failure("User not found", HttpStatusCode.NotFound);
        }
        
        string token = jwtService.GenerateToken(userId.Value, email, role);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsync(string email, string password)
    {
        Guid? userId = await authStore.RegisterAndGetUserIdAsync(email, password);
        
        if (userId == null)
        {
            return Result<string>.Failure("User already exists", HttpStatusCode.Conflict);
        }
        
        //todo
        string token = jwtService.GenerateToken(userId.Value, email, "user");
        
        return Result<string>.Success(token);
    }
}