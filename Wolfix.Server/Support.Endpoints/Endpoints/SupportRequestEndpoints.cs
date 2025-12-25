using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;
using Shared.Endpoints;
using Support.Application.Dto;
using Support.Application.Services;

namespace Support.Endpoints.Endpoints;

//TODO: CUSTOMER RESPONSES FOR REQUESTS

internal static class SupportRequestEndpoints
{
    private const string Route = "api/support-requests";

    public static void MapSupportRequestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(Route)
            .WithTags("Support Requests")
            .RequireAuthorization(Roles.Support);
        
        group.MapGet("", GetAllPending)
            .WithSummary("Get all support requests");
        
        app.MapPost(Route, Create)
            .WithTags("Support Requests")
            .RequireAuthorization(Roles.Customer)
            .WithSummary("Create support request");
        
        group.MapPatch("{supportRequestId:guid}/supports/{supportId:guid}/respond", Respond)
            .WithSummary("Respond on support request");
        
        group.MapPatch("{supportRequestId:guid}/supports/{supportId:guid}/cancel", Cancel)
            .WithSummary("Cancel support request");
        
        group.MapGet("by-category", GetAllByCategory)
            .WithSummary("Get all support requests by category")
            .RequireAuthorization("Support");
    }

    private static async Task<Ok<IReadOnlyCollection<SupportRequestShortDto>>> GetAllPending(
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        IReadOnlyCollection<SupportRequestShortDto> getAllRequestsResult = await supportRequestService.GetAllPendingAsync(ct);

        return TypedResults.Ok(getAllRequestsResult);
    }

    private static async Task<Results<NoContent, BadRequest<string>>> Create(
        [FromBody] CreateSupportRequestDto request,
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        VoidResult createRequestResult = await supportRequestService.CreateAsync(request, ct);

        if (createRequestResult.IsFailure)
        {
            return TypedResults.BadRequest(createRequestResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>>> Respond(
        [FromRoute] Guid supportId,
        [FromRoute] Guid supportRequestId,
        [FromBody] RespondOnRequestDto request,
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        VoidResult respondOnRequestResult = await supportRequestService.RespondAsync(supportId, supportRequestId, request, ct);

        if (respondOnRequestResult.IsFailure)
        {
            return TypedResults.NotFound(respondOnRequestResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>>> Cancel(
        [FromRoute] Guid supportId,
        [FromRoute] Guid supportRequestId,
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        VoidResult cancelRequestResult = await supportRequestService.CancelAsync(supportId, supportRequestId, ct);

        if (cancelRequestResult.IsFailure)
        {
            return TypedResults.NotFound(cancelRequestResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<IReadOnlyCollection<SupportRequestShortDto>>, BadRequest<string>>>
        GetAllByCategory(
            [FromBody] string category,
            [FromServices] SupportRequestService supportRequestService,
            CancellationToken ct)
    {
        Result<IReadOnlyCollection<SupportRequestShortDto>> result
            = await supportRequestService.GetAllByCategoryAsync(category, ct);
        
        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.ErrorMessage);
        }
        
        return TypedResults.Ok(result.Value);
    }
}