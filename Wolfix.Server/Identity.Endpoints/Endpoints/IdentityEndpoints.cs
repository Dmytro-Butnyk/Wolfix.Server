using System.Net;
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
    
    private static async Task<Results<Ok<string>, NotFound<string>, ForbidHttpResult>> LogIn(
        [FromBody] LogInDto logInDto,
        [FromQuery] string role,
        [FromServices] IAuthService authService)
    {
        Result<string> logInResult = await authService.LogInAsync(logInDto.Email, logInDto.Password, role);

        if (!logInResult.IsSuccess)
        {
            return logInResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(logInResult.ErrorMessage),
                HttpStatusCode.Forbidden => TypedResults.Forbid()
            };
        }
        
        return TypedResults.Ok(logInResult.Value);
    }

    private static async Task<Results<Ok<string>, Conflict<string>, InternalServerError<string>>> Register(
        [FromBody] RegisterDto registerDto,
        [FromServices] IAuthService authService)
    {
        Result<string> registerResult = await authService.RegisterAsync(registerDto.Email, registerDto.Password);

        if (!registerResult.IsSuccess)
        {
            return registerResult.StatusCode switch
            {
                HttpStatusCode.Conflict => TypedResults.Conflict(registerResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(registerResult.ErrorMessage)
            };
        }
        
        return TypedResults.Ok(registerResult.Value);
    }
}