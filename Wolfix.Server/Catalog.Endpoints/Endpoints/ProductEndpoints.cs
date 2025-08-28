using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.Review;
using Catalog.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Catalog.Endpoints.Endpoints;

internal static class ProductEndpoints
{
    private const string Route = "api/products";

    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var productGroup = app.MapGroup(Route)
            .WithTags("Products");
        
        MapGetEndpoints(productGroup);
    }

    private static void MapGetEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("category/{childCategoryId:guid}/page/{page:int}", GetAllByCategoryForPage);
        group.MapGet("with-discount/page/{page:int}", GetWithDiscountForPage);
        group.MapGet("recommended", GetRecommendedForPage);
        group.MapGet("random", GetRandom);
        group.MapGet("{productId:guid}/reviews", GetProductReviews);
    }

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, BadRequest<string>, NotFound<string>>> GetAllByCategoryForPage(
        [FromRoute] Guid childCategoryId,
        [FromRoute] int page,
        [FromServices] IProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1)
        {
            return TypedResults.BadRequest("Page must be greater than 0");
        }
        
        Result<PaginationDto<ProductShortDto>> getProductsByCategoryResult =
            await productService.GetForPageByCategoryIdAsync(childCategoryId, page, pageSize, ct);

        if (!getProductsByCategoryResult.IsSuccess)
        {
            return TypedResults.NotFound(getProductsByCategoryResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getProductsByCategoryResult.Value);
    }

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, BadRequest<string>>> GetWithDiscountForPage(
        [FromRoute] int page,
        [FromServices] IProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 4)
    {
        if (page < 1)
        {
            return TypedResults.BadRequest("Page must be greater than 0");
        }

        Result<PaginationDto<ProductShortDto>> getProductsWithDiscountResult =
            await productService.GetForPageWithDiscountAsync(page, pageSize, ct);
        
        return TypedResults.Ok(getProductsWithDiscountResult.Value);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<ProductShortDto>>, BadRequest<string>, NotFound<string>>>
        GetRecommendedForPage(
            [FromQuery] Guid[] visitedCategoriesIds,
            [FromServices] IProductService productService,
            CancellationToken ct,
            [FromQuery] int pageSize = 12)
    {
        if (visitedCategoriesIds.Length == 0)
        {
            return TypedResults.BadRequest("Visited categories must be not empty");
        }
        
        Result<IReadOnlyCollection<ProductShortDto>> getRecommendedProductsResult =
            await productService.GetRecommendedForPageAsync(pageSize, visitedCategoriesIds.ToList(), ct);

        if (!getRecommendedProductsResult.IsSuccess)
        {
            return TypedResults.NotFound(getRecommendedProductsResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getRecommendedProductsResult.Value);
    }

    private static async Task<Ok<IReadOnlyCollection<ProductShortDto>>> GetRandom(
        [FromServices] IProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 12)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getRandomProductsResult =
            await productService.GetRandomProductsAsync(pageSize, ct);
        
        return TypedResults.Ok(getRandomProductsResult.Value);
    }

    private static async Task<Results<Ok<CursorPaginationDto<ProductReviewDto>>, NotFound<string>>> GetProductReviews(
        [FromRoute] Guid productId,
        [FromServices] IProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? lastId = null)
    {
        Result<CursorPaginationDto<ProductReviewDto>> getProductReviewsResult =
            await productService.GetProductReviewsAsync(productId, pageSize, lastId, ct);
        
        if (!getProductReviewsResult.IsSuccess)
        {
            return TypedResults.NotFound(getProductReviewsResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getProductReviewsResult.Value);
    }
}