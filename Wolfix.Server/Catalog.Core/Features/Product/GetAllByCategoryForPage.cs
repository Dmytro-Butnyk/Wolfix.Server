using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Dto;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class GetAllByCategoryForPage
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
            app.MapGet("category/{childCategoryId:guid}/page/{page:int}", Handle)
                .WithSummary("Get all products by specific category for page with pagination");
        }

        private static async Task<Results<Ok<PaginationDto<Response>>, BadRequest<string>, NotFound<string>>> Handle(
            [FromRoute] Guid childCategoryId,
            [FromRoute] int page,
            [FromServices] CatalogContext db,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1)
            {
                return TypedResults.BadRequest("Page must be greater than 0");
            }

            //todo: добавить проверку на существование категории через доменный сервис(или событие) и кинуть нот фаунт если нету

            int totalCount = await db.Products
                .AsNoTracking()
                .Where(product => product.CategoryId == childCategoryId)
                .CountAsync(ct);

            if (totalCount == 0)
            {
                return TypedResults.Ok(PaginationDto<Response>.Empty(page));
            }

            List<Response> productsByCategory = await db.Products
                .Include("Discount")
                .Include("_productMedias")
                .AsNoTracking()
                .Where(product => product.CategoryId == childCategoryId)
                .OrderBy(product => product.FinalPrice)
                .Skip((page - 1) * pageSize)
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

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            PaginationDto<Response> paginationDto = new(
                page,
                totalPages,
                totalCount,
                productsByCategory
            );

            return TypedResults.Ok(paginationDto);
        }
    }
}