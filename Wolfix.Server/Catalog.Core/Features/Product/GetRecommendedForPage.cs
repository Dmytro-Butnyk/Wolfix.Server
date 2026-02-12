using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class GetRecommendedForPage
{
    public sealed record Response(
        Guid Id,
        string Title,
        double? AverageRating,
        decimal Price,
        decimal FinalPrice,
        uint Bonuses,
        string? MainPhoto
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("recommended", Handle)
                .WithSummary("Get recommended products by visitedCategories list for page with pagination");
        }

        private static async Task<Results<Ok<List<Response>>, NotFound<string>>> Handle(
            [FromQuery] Guid[] visitedCategoriesIds,
            [FromServices] CatalogContext db,
            CancellationToken ct,
            [FromQuery] int pageSize = 12)
        {
            if (visitedCategoriesIds.Length == 0)
            {
                return TypedResults.Ok(new List<Response>());
            }
            //todo: добавить проверку на существование категорий через доменный сервис(или событие) и кинуть нот фаунт если нету

            List<Response> recommendedProducts = new(pageSize);

            int productsByCategorySize = pageSize / visitedCategoriesIds.Length;
            int remainder = pageSize % visitedCategoriesIds.Length;

            for (var i = 0; i < visitedCategoriesIds.Length; ++i)
            {
                int count = productsByCategorySize + (i < remainder ? 1 : 0);
                Guid id = visitedCategoriesIds[i];

                var recommendedByCategory = await db.Products
                    .AsNoTracking()
                    .Include(p => p.Discount)
                    .Include("_productMedias")
                    .Where(product => product.CategoryId == id)
                    .OrderBy(_ => EF.Functions.Random())
                    .Take(count)
                    .Select(product => new Response(
                        product.Id,
                        product.Title,
                        product.AverageRating,
                        product.Price,
                        product.FinalPrice,
                        product.Bonuses,
                        product.MainPhotoUrl))
                    .ToListAsync(ct);

                recommendedProducts.AddRange(recommendedByCategory);
            }

            if (recommendedProducts.Count == 0)
            {
                return TypedResults.NotFound("Recommended products not found");
            }

            return TypedResults.Ok(recommendedProducts);
        }
    }
}