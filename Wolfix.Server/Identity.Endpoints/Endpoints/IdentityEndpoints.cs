using System.Net;
using Google.Apis.Auth;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Identity.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Models;
using Shared.Endpoints;
using Shared.Endpoints.Exceptions;
using GooglePayload = Google.Apis.Auth.GoogleJsonWebSignature.Payload;

namespace Identity.Endpoints.Endpoints;

internal static class IdentityEndpoints
{
    private const string Route = "api/account";
    
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var identityGroup = app.MapGroup(Route)
            .WithTags("Identity");
        
        identityGroup.MapPost("roles", LogInAndGetUserRoles)
            .WithSummary("Log in and get all roles");
        
        identityGroup.MapPost("token", GetTokenByRole)
            .WithSummary("Get token by specific role");

        var customerGroup = identityGroup.MapGroup("customer");
        MapCustomerEndpoints(customerGroup);

         var changeGroup = identityGroup.MapGroup("{accountId:guid}");
        MapChangeEndpoints(changeGroup);
    }

    private static void MapCustomerEndpoints(RouteGroupBuilder customerGroup)
    {
        customerGroup.MapPost("register", Register)
            .WithSummary("Register as customer");
        
        customerGroup.MapPatch("google", ContinueWithGoogle)
            .WithSummary("Continue with google");
    }

    private static void MapChangeEndpoints(RouteGroupBuilder group)
    {
        group.MapPatch("email", ChangeEmail)
            .RequireAuthorization(Roles.Customer, Roles.Seller)
            .WithSummary("Change email");
        
        group.MapPatch("password", ChangePassword)
            .RequireAuthorization(Roles.Customer, Roles.Seller)
            .WithSummary("Change password");
    }

    private static async Task<Results<Ok<string>, Conflict<string>, BadRequest<string>, InternalServerError<string>, NotFound<string>>>
        ContinueWithGoogle([FromBody] GoogleLoginDto request,
        [FromServices] IConfiguration configuration,
        [FromServices] AuthService authService,
        CancellationToken ct)
    {
        GooglePayload? payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [configuration["GOOGLE_CLIENT_ID"]]
        });

        if (payload is null)
        {
            return TypedResults.BadRequest("Invalid token");
        }

        Result<string> getTokenResult = await authService.ContinueWithGoogleAsync(payload, ct);

        if (getTokenResult.IsFailure)
        {
            return getTokenResult.StatusCode switch
            {
                HttpStatusCode.Conflict => TypedResults.Conflict(getTokenResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(getTokenResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(getTokenResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(getTokenResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(ContinueWithGoogle), getTokenResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(getTokenResult.Value);
    }
    
    private static async Task<Results<Ok<UserRolesDto>, NotFound<string>, BadRequest<string>, InternalServerError<string>, ForbidHttpResult>>
        LogInAndGetUserRoles(
        [FromBody] LogInDto logInDto,
        [FromServices] AuthService authService,
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
                HttpStatusCode.Forbidden => TypedResults.Forbid(),
                _ => throw new UnknownStatusCodeException(nameof(LogInAndGetUserRoles), logInResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(logInResult.Value);
    }

    private static async Task<Results<Ok<string>, NotFound<string>, ForbidHttpResult, BadRequest<string>>> GetTokenByRole(
        [FromBody] TokenDto tokenDto,
        [FromServices] AuthService authService,
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
                _ => throw new UnknownStatusCodeException(nameof(GetTokenByRole), getTokenResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(getTokenResult.Value);
    }

    private static async Task<Results<Ok<string>, Conflict<string>, InternalServerError<string>, BadRequest<string>>> Register(
        [FromBody] RegisterAsCustomerDto registerAsCustomerDto,
        [FromServices] AuthService authService,
        CancellationToken ct)
    {
        Result<string> registerResult = await authService.RegisterAsync(registerAsCustomerDto, ct);

        if (!registerResult.IsSuccess)
        {
            return registerResult.StatusCode switch
            {
                HttpStatusCode.Conflict => TypedResults.Conflict(registerResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(registerResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(registerResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(Register), registerResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(registerResult.Value);
    }

    private static async Task<Results<Ok<string>, UnauthorizedHttpResult, NotFound<string>, BadRequest<string>>> ChangeEmail(
        [FromBody] ChangeEmailDto request,
        [FromRoute] Guid accountId,
        [FromServices] AuthService authService,
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
                _ => throw new UnknownStatusCodeException(nameof(ChangeEmail), changeEmailResult.StatusCode)
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
            [FromServices] AuthService authService,
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
                _ => throw new UnknownStatusCodeException(nameof(ChangePassword), changePasswordResult.StatusCode)
            };
        }
        
        return TypedResults.NoContent();
    }
}