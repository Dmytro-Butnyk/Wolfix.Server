using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class DeleteProduct
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/products/{productId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithTags("Products")
                .WithSummary("Delete product");
        }

        private static async Task<Results<NoContent, NotFound<string>>> Handle(
            [FromRoute] Guid productId,
            [FromServices] CatalogContext db,
            CancellationToken cancellationToken)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            // var @event = new DeleteProductMedia
            // {
            //     MediaUrl = product.MainPhotoUrl
            // };
            
            //todo: пофиксить (ажур аккаунт срок истёк)
            
            // VoidResult deleteProductMediaResult = await eventBus.PublishWithoutResultAsync(@event, ct);
            //
            // if (deleteProductMediaResult.IsFailure)
            // {
            //     return deleteProductMediaResult;
            // }
            
            db.Products.Remove(product);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        }
    }
}