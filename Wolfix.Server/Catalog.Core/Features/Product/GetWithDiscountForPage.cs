using Catalog.Domain.ProductAggregate.Enums;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Dto;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class GetWithDiscountForPage
{
    public sealed record Response(Guid Id, string Title, double? AverageRating,
        decimal Price, decimal FinalPrice, uint Bonuses, string? MainPhoto);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("with-discount/page/{page:int}", Handle)
                .WithSummary("Get all products with discount for page with pagination");
        }

        private static async Task<Results<Ok<PaginationDto<Response>>, BadRequest<string>>> Handle(
            [FromRoute] int page,
            [FromServices] CatalogContext db,
            CancellationToken ct,
            [FromQuery] int pageSize = 4)
        {
            if (page < 1)
            {
                return TypedResults.BadRequest("Page must be greater than 0");
            }

            var totalCount = await db.Products
                .AsNoTracking()
                .Include(p => p.Discount)
                .CountAsync(p => p.Discount != null && p.Discount.Status == DiscountStatus.Active, ct);

            if (totalCount == 0)
            {
                return TypedResults.Ok(PaginationDto<Response>.Empty(page));
            }

            var productsWithDiscount = await db.Products
                .AsNoTracking()
                .Include(p => p.Discount)
                .Include("_productMedias")
                .Where(p => p.Discount != null && p.Discount.Status == DiscountStatus.Active)
                .OrderByDescending(p => p.Discount!.Percent)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new Response(p.Id, p.Title, p.AverageRating, p.Price, p.FinalPrice,
                    p.Bonuses, p.MainPhotoUrl))
                .ToListAsync(ct);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            PaginationDto<Response> paginationDto = new(page, totalPages, totalCount, productsWithDiscount);

            return TypedResults.Ok(paginationDto);
        }
    }
}