using Catalog.Domain.Services;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Domain.Models;

namespace Catalog.Core.Features.Product;

public static class GetSearchByCategory
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
            app.MapGet("search/category/{categoryId:guid}", Handle)
                .WithSummary("Get products by search query and category");
        }

        private static async Task<Results<Ok<List<Response>>, BadRequest<string>, NotFound<string>>> Handle(
            [FromRoute] Guid categoryId,
            [FromQuery] string searchQuery,
            [FromServices] CatalogContext db,
            [FromServices] ProductDomainService productDomainService,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return TypedResults.BadRequest("Search query is null or empty");
            }

            VoidResult isCategoryExist = await productDomainService.IsCategoryExistAsync(categoryId, ct);

            if (isCategoryExist.IsFailure)
            {
                return TypedResults.NotFound(isCategoryExist.ErrorMessage);
            }

            const double threshold = 0.30;

            List<Response> products = await db.Products
                .Include(p => p.Discount)
                .Include("_productMedias")
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId &&
                            EF.Functions.TrigramsSimilarity(p.Title, searchQuery) > threshold)
                .OrderByDescending(p => EF.Functions.TrigramsSimilarity(p.Title, searchQuery))
                .Take(pageSize)
                .Select(product => new Response(
                    product.Id,
                    product.Title,
                    product.AverageRating,
                    product.Price,
                    product.FinalPrice,
                    product.Bonuses,
                    product.MainPhotoUrl
                ))
                .ToListAsync(ct);

            return TypedResults.Ok(products);
        }
    }
}