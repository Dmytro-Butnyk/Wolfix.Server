using System.Net;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;

namespace Catalog.Core.Features.Product;

public static class DeleteDiscount
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("product/{productId:guid}/discount", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Delete product discount");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromRoute] Guid productId,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            var product = await db.Products
                .Include("Discount")
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"product with id: {productId} not found");
            }

            var deleteDiscountResult = product.RemoveDiscount();

            if (deleteDiscountResult.IsFailure)
            {
                return TypedResults.BadRequest(deleteDiscountResult.ErrorMessage);
            }

            await db.SaveChangesAsync(ct);

            return TypedResults.NoContent();
        }
    }
}