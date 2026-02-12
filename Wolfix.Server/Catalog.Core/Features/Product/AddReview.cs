using System.Net;
using Catalog.Application.Contracts;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Product;

public static class AddReview
{
    public sealed record Request(string Title, string Text, uint Rating, Guid CustomerId);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("{productId:guid}/reviews", Handle)
                .RequireAuthorization(AuthorizationRoles.Customer)
                .WithSummary("Add review");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromBody] Request request,
            [FromRoute] Guid productId,
            [FromServices] CatalogContext db,
            [FromServices] EventBus eventBus,
            [FromServices] IToxicityService toxicityService,
            CancellationToken ct)
        {
            Domain.ProductAggregate.Product? product = await db.Products
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            VoidResult result = await eventBus.PublishWithoutResultAsync(new CheckCustomerExistsForAddingReview
            {
                CustomerId = request.CustomerId
            }, ct);

            if (result.IsFailure)
            {
                return TypedResults.NotFound(result.ErrorMessage);
            }

            Result<bool> checkToxicityResult = await toxicityService.IsToxic(request.Text, ct);

            if (checkToxicityResult.IsFailure)
            {
                return TypedResults.BadRequest(checkToxicityResult.ErrorMessage);
            }

            bool isToxic = checkToxicityResult.Value;

            if (isToxic)
            {
                return TypedResults.BadRequest("Review is toxic");
            }

            VoidResult addProductReviewResult = product.AddReview(request.Title, request.Text, request.Rating, request.CustomerId);

            if (addProductReviewResult.IsFailure)
            {
                return TypedResults.BadRequest(addProductReviewResult.ErrorMessage);
            }

            await db.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }
}