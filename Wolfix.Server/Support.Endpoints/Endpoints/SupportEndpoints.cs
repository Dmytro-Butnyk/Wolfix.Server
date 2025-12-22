using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;
using Support.Application.Dto;
using Support.Application.Services;

namespace Support.Endpoints.Endpoints;

internal static class SupportEndpoints
{
    private const string Route = "api/supports";

    public static void MapSupportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Route)
            .WithTags("Support");
        
        group.MapPost("", CreateSupport)
            .RequireAuthorization("SuperAdmin")
            .WithSummary("Create support request");
    }
    
    private static async Task<Results<NoContent, BadRequest<string>, Conflict<string>, InternalServerError<string>>> CreateSupport(
        [FromBody] CreateSupportDto request,
        [FromServices] SupportService supportService,
        CancellationToken ct)
    {
        VoidResult createSupportResult = await supportService.CreateSupportAsync(request, ct);

        if (createSupportResult.IsFailure)
        {
            return createSupportResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(createSupportResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(createSupportResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(createSupportResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(CreateSupport)} -> Unknown status code: {createSupportResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }
}