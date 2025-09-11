using System.Net;
using Identity.Application.Dto;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Identity.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
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

        identityGroup.MapPost("seller/register", RegisterAsSeller)
            .DisableAntiforgery()
            .WithSummary("Register as seller");
        
        var changeGroup = identityGroup.MapGroup("{accountId:guid}");
        MapChangeEndpoints(changeGroup);
    }

    private static void MapCustomerEndpoints(RouteGroupBuilder customerGroup)
    {
        customerGroup.MapPost("roles", LogInAndGetUserRoles)
            .WithSummary("Log in and get all roles");
        
        customerGroup.MapPost("token", GetTokenByRole)
            .WithSummary("Get token by specific role");
        
        customerGroup.MapPost("register", RegisterAsCustomer)
            .WithSummary("Register as customer");
    }

    private static void MapChangeEndpoints(RouteGroupBuilder group)
    {
        group.MapPatch("email", ChangeEmail)
            .WithSummary("Change email");
        
        group.MapPatch("password", ChangePassword)
            .WithSummary("Change password");
    }
    
    private static async Task<Results<Ok<UserRolesDto>, NotFound<string>, BadRequest<string>, InternalServerError<string>>> LogInAndGetUserRoles(
        [FromBody] LogInDto logInDto,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        Result<UserRolesDto> logInResult = await authService.LogInAndGetUserRolesAsync(logInDto, ct);

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

    private static async Task<Results<Ok<string>, NotFound<string>, ForbidHttpResult, BadRequest<string>>> GetTokenByRole(
        [FromBody] TokenDto tokenDto,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        Result<string> getTokenResult = await authService.GetTokenByRoleAsync(tokenDto, ct);

        if (!getTokenResult.IsSuccess)
        {
            return getTokenResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(getTokenResult.ErrorMessage),
                HttpStatusCode.Forbidden => TypedResults.Forbid(),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(getTokenResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(getTokenResult.Value);
    }

    private static async Task<Results<Ok<string>, Conflict<string>, InternalServerError<string>, BadRequest<string>>> RegisterAsCustomer(
        [FromBody] RegisterAsCustomerDto registerAsCustomerDto,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        Result<string> registerResult = await authService.RegisterAsCustomerAsync(registerAsCustomerDto, ct);

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

    private static async Task<Results<Ok<string>, BadRequest<string>, Conflict<string>, InternalServerError<string>>> RegisterAsSeller(
        [FromForm] RegisterAsSellerDto registerAsSellerDto,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        Result<string> registerResult = await authService.RegisterAsSellerAsync(registerAsSellerDto, ct);

        if (!registerResult.IsSuccess)
        {
            string errorMessage = registerResult.ErrorMessage!;

            return registerResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(errorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(errorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(errorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(registerResult.Value);
    }

    private static async Task<Results<Ok<string>, UnauthorizedHttpResult, NotFound<string>, BadRequest<string>>> ChangeEmail(
        [FromBody] ChangeEmailDto request,
        [FromRoute] Guid accountId,
        [FromServices] IAuthService authService,
        HttpContext context,
        CancellationToken ct)
    {
        string? token = GetToken(context);

        if (token is null)
        {
            return TypedResults.Unauthorized();
        }
        
        Result<string> changeEmailResult = await authService.ChangeEmailAsync(accountId, request, token, ct);

        if (!changeEmailResult.IsSuccess)
        {
            return changeEmailResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeEmailResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeEmailResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(changeEmailResult.Value);
    }
    
    private static string? GetToken(HttpContext context)
    {
        string? authHeader = context.Request.Headers.Authorization;

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return null;
        }

        return authHeader["Bearer ".Length..];
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> ChangePassword(
            [FromBody] ChangePasswordDto request,
            [FromRoute] Guid accountId,
            [FromServices] IAuthService authService,
            CancellationToken ct)
    {
        if (request.CurrentPassword == request.NewPassword)
        {
            return TypedResults.BadRequest("New password must be different from current password");
        }
        
        VoidResult changePasswordResult = await authService.ChangePasswordAsync(accountId, request, ct);

        if (!changePasswordResult.IsSuccess)
        {
            return changePasswordResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changePasswordResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changePasswordResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.NoContent();
    }
}