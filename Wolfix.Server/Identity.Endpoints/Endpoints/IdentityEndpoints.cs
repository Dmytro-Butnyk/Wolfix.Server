using System.Net;
using Identity.Application.Dto;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
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

        var customerGroup = identityGroup.MapGroup("customer");
        MapCustomerEndpoints(customerGroup);
    }

    private static void MapCustomerEndpoints(RouteGroupBuilder customerGroup)
    {
        customerGroup.MapPost("roles", LogInAndGetUserRoles);
        customerGroup.MapPost("token", GetTokenByRole);
        customerGroup.MapPost("register", RegisterAsCustomer);
    }
    
    private static async Task<Results<Ok<UserRolesDto>, NotFound<string>, BadRequest<string>, InternalServerError<string>>> LogInAndGetUserRoles(
        [FromBody] LogInDto logInDto,
        [FromServices] IAuthService authService)
    {
        Result<UserRolesDto> logInResult = await authService.LogInAndGetUserRolesAsync(logInDto.Email, logInDto.Password);

        if (!logInResult.IsSuccess)
        {
            return logInResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(logInResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(logInResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(logInResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(logInResult.Value);
    }

    private static async Task<Results<Ok<string>, NotFound<string>, ForbidHttpResult>> GetTokenByRole(
        [FromBody] TokenDto tokenDto,
        [FromServices] IAuthService authService)
    {
        Result<string> getTokenResult = await authService.GetTokenByRoleAsync(tokenDto.UserId, tokenDto.Email, tokenDto.Role);

        if (!getTokenResult.IsSuccess)
        {
            return getTokenResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(getTokenResult.ErrorMessage),
                HttpStatusCode.Forbidden => TypedResults.Forbid(),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(getTokenResult.Value);
    }

    private static async Task<Results<Ok<string>, Conflict<string>, InternalServerError<string>, BadRequest<string>>> RegisterAsCustomer(
        [FromBody] RegisterDto registerDto,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        Result<string> registerResult = await authService.RegisterAsCustomerAsync(registerDto.Email, registerDto.Password, ct);

        if (!registerResult.IsSuccess)
        {
            return registerResult.StatusCode switch
            {
                HttpStatusCode.Conflict => TypedResults.Conflict(registerResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(registerResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(registerResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(registerResult.Value);
    }
}