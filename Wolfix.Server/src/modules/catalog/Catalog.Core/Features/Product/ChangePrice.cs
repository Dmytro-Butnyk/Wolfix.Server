using System.Net;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;

namespace Catalog.Core.Features.Product;

public static class ChangePrice
{
    public sealed record Request(decimal Price);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("product/{productId:guid}/price", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Change product price");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromRoute] Guid productId,
            [FromBody] Request request,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            var product = await db.Products
                .Include("Discount")
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            var changePriceResult = product.ChangePrice(request.Price);

            if (changePriceResult.IsFailure)
            {
                return TypedResults.BadRequest(changePriceResult.ErrorMessage);
            }

            await db.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }
}