using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;

namespace Catalog.Core.Features.Product;

public static class AddDiscount
{
    public sealed record Request(uint Percent, DateTime ExpirationDateTime);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("product/{productId:guid}/discount", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Add discount to product");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> Handle(
            [FromRoute] Guid productId,
            [FromBody] Request request,
            [FromServices] CatalogContext db,
            CancellationToken ct)
        {
            Domain.ProductAggregate.Product? product = await db.Products
                .Include("Discount")
                .FirstOrDefaultAsync(p => p.Id == productId, ct);

            if (product is null)
            {
                return TypedResults.NotFound($"product with id: {productId} not found");
            }
            
            VoidResult addDiscountResult = product.AddDiscount(request.Percent, request.ExpirationDateTime);

            if (addDiscountResult.IsFailure)
            {
                return TypedResults.BadRequest(addDiscountResult.ErrorMessage);
            }

            await db.SaveChangesAsync(ct);
            
            return TypedResults.NoContent();
        }
    }
}