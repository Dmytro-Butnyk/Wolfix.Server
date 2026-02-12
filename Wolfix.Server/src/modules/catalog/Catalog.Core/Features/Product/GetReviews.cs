using Catalog.Domain.ProductAggregate.Entities;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Dto;
using Shared.Core.Endpoints;

namespace Catalog.Core.Features.Product;

public static class GetReviews
{
    public sealed record Response(
        Guid Id,
        string Title,
        string Text,
        uint Rating,
        Guid ProductId,
        DateTime CreatedAt
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("{productId:guid}/reviews", Handle)
                .WithSummary("Get all reviews by specific product");
        }

        private static async Task<Results<Ok<CursorPaginationDto<Response>>, NotFound<string>>> Handle(
            [FromRoute] Guid productId,
            [FromServices] CatalogContext db,
            CancellationToken ct,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? lastId = null)
        {
            bool productExists = await db.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == productId, ct);

            if (!productExists)
            {
                return TypedResults.NotFound($"Product with id: {productId} not found");
            }

            IQueryable<Review> query = db.Products
                .AsNoTracking()
                .Include("_reviews")
                .Where(p => p.Id == productId)
                .SelectMany(p => EF.Property<List<Review>>(p, "_reviews"));

            if (lastId.HasValue)
            {
                query = query.Where(review => review.Id.CompareTo(lastId) > 0);
            }

            List<Response> productReviewsDto = await query
                .OrderBy(review => review.CreatedAt)
                .Take(pageSize)
                .Select(review => new Response(
                    review.Id,
                    review.Title,
                    review.Text,
                    review.Rating,
                    review.ProductId,
                    review.CreatedAt
                ))
                .ToListAsync(ct);
            
            Guid? nextCursor = productReviewsDto.Count > 0 ? productReviewsDto.Last().Id : null;
            
            CursorPaginationDto<Response> cursorPaginationDto = new(productReviewsDto, nextCursor);

            return TypedResults.Ok(cursorPaginationDto);
        }
    }
}