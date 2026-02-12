using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class GetRandom
{
    public sealed record Response(Guid Id, string Title, double? AverageRating, decimal Price,
        decimal FinalPrice, uint Bonuses, string? MainPhoto);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("random", Handle)
                .WithSummary("Get random products");
        }

        private static async Task<Ok<List<Response>>> Handle(
            [FromServices] CatalogContext db,
            CancellationToken ct,
            [FromQuery] int pageSize = 12)
        {
            var productCount = await db.Products
                .AsNoTracking()
                .CountAsync(ct);
            
            if (productCount == 0)
            {
                return TypedResults.Ok(new List<Response>());
            }
            
            var randomProducts = await db.Products
                .AsNoTracking()
                .Include(p => p.Discount)
                .Include("_productMedias")
                .OrderBy(_ => EF.Functions.Random())
                .Take(pageSize)
                .Select(p => new Response(
                    p.Id,
                    p.Title,
                    p.AverageRating,
                    p.Price,
                    p.FinalPrice,
                    p.Bonuses,
                    p.MainPhotoUrl))
                .ToListAsync(ct);

            return TypedResults.Ok(randomProducts);
        }
    }
}