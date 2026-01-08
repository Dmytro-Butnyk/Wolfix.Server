using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;
using Shared.Endpoints;
using Shared.Endpoints.Exceptions;
using Support.Application.Dto;
using Support.Application.Dto.SupportRequest;
using Support.Application.Dto.SupportRequest.Create;
using Support.Application.Services;

namespace Support.Endpoints.Endpoints;

internal static class SupportRequestEndpoints
{
    private const string Route = "api/support-requests";

    public static void MapSupportRequestEndpoints(this IEndpointRouteBuilder app)
    {
        var supportGroup = app.MapGroup(Route)
            .WithTags("Support Requests")
            .RequireAuthorization(AuthorizationRoles.Support);
        
        supportGroup.MapGet("", GetAllPending)
            .WithSummary("Get all support requests");
        
        supportGroup.MapPatch("{supportRequestId:guid}/supports/{supportId:guid}/respond", Respond)
            .WithSummary("Respond on support request");
        
        supportGroup.MapPatch("{supportRequestId:guid}/supports/{supportId:guid}/cancel", Cancel)
            .WithSummary("Cancel support request");

        supportGroup.MapGet("by-category", GetAllByCategory)
            .WithSummary("Get all support requests by category");

        var customerGroup = app.MapGroup(Route)
            .WithTags("Support Requests")
            .RequireAuthorization(AuthorizationRoles.Customer);
        
        customerGroup.MapPost(Route, Create)
            .WithSummary("Create support request");

        customerGroup.MapGet("{customerId:guid}", GetAllForCustomer)
            .WithSummary("Get all support requests for customer");
        
        customerGroup.MapGet("{customerId:guid}/{supportRequestId:guid}", GetForCustomer)
            .WithSummary("Get support request full info for customer");
    }

    private static async Task<Ok<IReadOnlyCollection<SupportRequestShortDto>>> GetAllPending(
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        IReadOnlyCollection<SupportRequestShortDto> getAllRequestsResult = await supportRequestService.GetAllPendingAsync(ct);

        return TypedResults.Ok(getAllRequestsResult);
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Create(
        [FromBody] CreateSupportRequestDto request,
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        VoidResult createRequestResult = await supportRequestService.CreateAsync(request, ct);

        if (createRequestResult.IsFailure)
        {
            return createRequestResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(createRequestResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(createRequestResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(
                    nameof(SupportRequestEndpoints),
                    nameof(Create),
                    createRequestResult.StatusCode
                )
            };
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

    //TODO: убрать этот метод и просто добавить [FromQuery] параметр в базовый ендпоинт
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

    private static async Task<Results<Ok<IReadOnlyCollection<SupportRequestForCustomerShortDto>>, NotFound<string>, BadRequest<string>>>
        GetAllForCustomer(
            [FromRoute] Guid customerId,
            [FromServices] SupportRequestService supportRequestService,
            CancellationToken ct,
            [FromQuery] string? category = null)
    {
        Result<IReadOnlyCollection<SupportRequestForCustomerShortDto>> result =
            await supportRequestService.GetAllForCustomerAsync(customerId, ct, category);

        if (result.IsFailure)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(result.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(result.ErrorMessage),
                _ => throw new UnknownStatusCodeException(
                    nameof(SupportRequestEndpoints),
                    nameof(GetAllForCustomer), 
                    result.StatusCode
                )
            };
        }
        
        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<SupportRequestForCustomerDto>, NotFound<string>, BadRequest<string>>> GetForCustomer(
        [FromRoute] Guid customerId,
        [FromRoute] Guid supportRequestId,
        [FromServices] SupportRequestService supportRequestService,
        CancellationToken ct)
    {
        Result<SupportRequestForCustomerDto> result = await supportRequestService.GetForCustomerAsync(customerId, supportRequestId, ct);

        if (result.IsFailure)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(result.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(result.ErrorMessage),
                _ => throw new UnknownStatusCodeException(
                    nameof(SupportRequestEndpoints),
                    nameof(GetForCustomer),
                    result.StatusCode
                )
            };
        }
        
        return TypedResults.Ok(result.Value);
    }
}