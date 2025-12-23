using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Application.Dto;
using Shared.Domain.Models;
using Shared.Endpoints.Exceptions;
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
        
        group.MapGet("page/{page:int}", GetAllForPage)
            .RequireAuthorization("SuperAdmin")
            .WithSummary("Get all supports for page");
        
        group.MapPost("", Create)
            .RequireAuthorization("SuperAdmin")
            .WithSummary("Create support");
        
        group.MapDelete("{supportId:guid}", Delete)
            .RequireAuthorization("SuperAdmin")
            .WithSummary("Delete support");
    }

    private static async Task<Ok<PaginationDto<SupportForAdminDto>>> GetAllForPage(
        [FromRoute] int page,
        [FromServices] SupportService supportService,
        CancellationToken ct,
        [FromQuery] int pageSize = 50)
    {
        PaginationDto<SupportForAdminDto> dto = await supportService.GetForPageAsync(page, pageSize, ct);
        
        return TypedResults.Ok(dto);
    }
    
    private static async Task<Results<NoContent, BadRequest<string>, Conflict<string>, InternalServerError<string>>> Create(
        [FromBody] CreateSupportDto request,
        [FromServices] SupportService supportService,
        CancellationToken ct)
    {
        VoidResult createSupportResult = await supportService.CreateAsync(request, ct);

        if (createSupportResult.IsFailure)
        {
            return createSupportResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(createSupportResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(createSupportResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(createSupportResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(Create), createSupportResult.StatusCode)
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>, InternalServerError<string>>> Delete(
        [FromRoute] Guid supportId,
        [FromServices] SupportService supportService,
        CancellationToken ct)
    {
        VoidResult deleteResult = await supportService.DeleteAsync(supportId, ct);

        if (deleteResult.IsFailure)
        {
            return deleteResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(deleteResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(deleteResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(Delete), deleteResult.StatusCode)
            };
        }
        
        return TypedResults.NoContent();
    }
}