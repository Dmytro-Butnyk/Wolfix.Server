using Catalog.Domain.Services;
using Catalog.Infrastructure;
using Catalog.IntegrationEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Dto;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using Shared.IntegrationEvents;

namespace Catalog.Core.Features.Product;

public static class GetAllBySellerCategoryForPage
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
            app.MapGet("seller/{sellerId:guid}/category/{categoryId:guid}/page/{page:int}", Handle)
                .RequireAuthorization(AuthorizationRoles.Seller)
                .WithSummary("Get all products by specific seller category");
        }

        private static async Task<Results<Ok<PaginationDto<Response>>, NotFound<string>>> Handle(
            [FromRoute] Guid sellerId,
            [FromRoute] Guid categoryId,
            [FromRoute] int page,
            [FromServices] CatalogContext db,
            [FromServices] ProductDomainService productDomainService,
            [FromServices] EventBus eventBus,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
        {
            VoidResult checkCategoryExistResult = await productDomainService.IsCategoryExistAsync(categoryId, ct);

            if (checkCategoryExistResult.IsFailure)
            {
                return TypedResults.NotFound(checkCategoryExistResult.ErrorMessage);
            }

            VoidResult checkSellerExistResult = await eventBus.PublishWithoutResultAsync(
                new CheckSellerWithCategoryExist(sellerId, categoryId),ct
            );

            if (checkSellerExistResult.IsFailure)
            {
                return TypedResults.NotFound(checkSellerExistResult.ErrorMessage);
            }

            int totalCount = await db.Products
                .AsNoTracking()
                .CountAsync(product => product.SellerId == sellerId && product.CategoryId == categoryId, ct);

            if (totalCount == 0)
            {
                return TypedResults.Ok(PaginationDto<Response>.Empty(page));
            }

            List<Response> products = await db.Products
                .AsNoTracking()
                .Include("_reviews")
                .Where(product => product.SellerId == sellerId && product.CategoryId == categoryId)
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
                products
            );

            return TypedResults.Ok(paginationDto);
        }
    }
}