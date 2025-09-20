using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Seller.Application.Dto.SellerApplication;
using Seller.Application.Interfaces;
using Shared.Domain.Models;

namespace Seller.Endpoints.Endpoints;

internal static class SellerApplicationEndpoints
{
    private const string Route = "api/seller-applications";

    public static void MapSellerApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var sellerApplicationGroup = app.MapGroup(Route)
            .WithTags("Seller Application");
        
        sellerApplicationGroup.MapPost("{accountId:guid}", CreateApplication)
            .DisableAntiforgery()
            .WithSummary("Create application to be a seller");
        
        sellerApplicationGroup.MapGet("", GetAllPendingApplications)
            .WithSummary("Get all pending applications");
        
        sellerApplicationGroup.MapPatch("{sellerApplicationId:guid}/approve", ApproveApplication)
            .WithSummary("Approve application");
        
        sellerApplicationGroup.MapPatch("{sellerApplicationId:guid}/reject", RejectApplication)
            .WithSummary("Reject application");
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> CreateApplication(
        [FromForm] CreateSellerApplicationDto request,
        [FromRoute] Guid accountId,
        [FromServices] ISellerApplicationService sellerApplicationService,
        CancellationToken ct)
    {
        VoidResult createApplicationResult = await sellerApplicationService.CreateAsync(accountId, request, ct);

        if (createApplicationResult.IsFailure)
        {
            return createApplicationResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(createApplicationResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(createApplicationResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(CreateApplication)} -> Unknown status code: {createApplicationResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Ok<IReadOnlyCollection<SellerApplicationDto>>> GetAllPendingApplications(
        [FromServices] ISellerApplicationService sellerApplicationService,
        CancellationToken ct)
    {
        IReadOnlyCollection<SellerApplicationDto> pendingApplications =
            await sellerApplicationService.GetPendingApplicationsAsync(ct);
        
        return TypedResults.Ok(pendingApplications);
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>, InternalServerError<string>, Conflict<string>>>
        ApproveApplication(
            [FromRoute] Guid sellerApplicationId,
            [FromServices] ISellerApplicationService sellerApplicationService,
            CancellationToken ct)
    {
        VoidResult approveApplicationResult = await sellerApplicationService.ApproveApplicationAsync(sellerApplicationId, ct);

        if (approveApplicationResult.IsFailure)
        {
            return approveApplicationResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(approveApplicationResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(approveApplicationResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(approveApplicationResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(approveApplicationResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ApproveApplication)} -> Unknown status code: {approveApplicationResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> RejectApplication(
        [FromRoute] Guid sellerApplicationId,
        [FromServices] ISellerApplicationService sellerApplicationService,
        CancellationToken ct)
    {
        VoidResult rejectApplicationResult = await sellerApplicationService.RejectApplicationAsync(sellerApplicationId, ct);

        if (rejectApplicationResult.IsFailure)
        {
            return rejectApplicationResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(rejectApplicationResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(rejectApplicationResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(RejectApplication)} -> Unknown status code: {rejectApplicationResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }
}