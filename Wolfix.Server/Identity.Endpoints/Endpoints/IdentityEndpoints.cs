using Identity.Application.Dto;
using Identity.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;

namespace Identity.Endpoints.Endpoints;

internal static class IdentityEndpoints
{
    private const string Route = "api/account";
    
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var identityGroup = app.MapGroup(Route)
            .WithTags("Identity");

        identityGroup.MapPost("login", LogIn);
        identityGroup.MapPost("register", Register);
    }
    //todo: все прочекать от начала до конца, привести в порядок и разобраться!!!!!!!!!!!!
    private static async Task<Results<Ok<string>, NotFound<string>>> LogIn(
        [FromBody] LogInDto logInDto,
        [FromQuery] string role,
        CancellationToken ct,
        [FromServices] IAuthService authService)
    {
        Result<string> logInResult = await authService.LogInAsync(logInDto.Email, logInDto.Password, role);

        if (!logInResult.IsSuccess)
        {
            return TypedResults.NotFound(logInResult.ErrorMessage);
        }
        
        return TypedResults.Ok(logInResult.Value);
    }

    private static async Task<Results<Ok<string>, Conflict<string>>> Register(
        [FromBody] RegisterDto registerDto,
        CancellationToken ct,
        [FromServices] IAuthService authService)
    {
        Result<string> registerResult = await authService.RegisterAsync(registerDto.Email, registerDto.Password);

        if (!registerResult.IsSuccess)
        {
            return TypedResults.Conflict(registerResult.ErrorMessage);
        }
        
        return TypedResults.Ok(registerResult.Value);
    }
}