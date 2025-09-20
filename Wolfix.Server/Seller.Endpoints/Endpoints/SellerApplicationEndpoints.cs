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
                HttpStatusCode.BadRequest => TypedResults.BadRequest(createApplicationResult.ErrorMessage)
            };
        }
        
        return TypedResults.NoContent();
    }
}