using System.Net;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Product;

public static class DeleteProductMedia
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("product/{productId:guid}/media/{mediaId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Delete product media");
        }

        private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> Handle(
            [FromRoute] Guid productId,
            [FromRoute] Guid mediaId,
            [FromServices] CatalogContext db,
            [FromServices] EventBus eventBus,
            CancellationToken ct)
        {
            var product = await db.Products
                .Include("_productMedias")
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            Result<Guid> deleteProductMediaResult = product.RemoveProductMedia(mediaId);

            if (deleteProductMediaResult.IsFailure)
            {
                return deleteProductMediaResult.StatusCode switch
                {
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteProductMediaResult.ErrorMessage),
                    HttpStatusCode.NotFound => TypedResults.NotFound(deleteProductMediaResult.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        nameof(Product),
                        nameof(DeleteProductMedia),
                        deleteProductMediaResult.StatusCode
                    )
                };
            }

            await db.SaveChangesAsync(ct);

            VoidResult eventResult = await eventBus.PublishWithoutResultAsync(
                new ProductMediaDeleted(deleteProductMediaResult.Value), ct);

            if (eventResult.IsFailure)
            {
                return TypedResults.NotFound(eventResult.ErrorMessage);
            }

            return TypedResults.NoContent();
        }
    }
}