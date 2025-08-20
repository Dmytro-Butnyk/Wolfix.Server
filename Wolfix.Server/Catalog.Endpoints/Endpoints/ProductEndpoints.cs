using Catalog.Application.Dto.Product;
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
        group.MapGet("with-discount/page/{page:int}", GetProductsWithDiscountForPage);
        group.MapGet("recommended", GetRecommendedProductsForPage);
        group.MapGet("random", GetRandomProducts);
    }
    
    #region ADD PRODUCT ENDPOINTS HERE

    private static async Task<VoidResult> AddProduct(
        
        )
    {
        
        
        return VoidResult.Success();
    }
    
    #endregion

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, NotFound<string>>> GetAllByCategoryForPage(
        [FromRoute] Guid childCategoryId,
        [FromRoute] int page,
        [FromQuery] int pageSize,
        CancellationToken ct,
        [FromServices] IProductService productService)
    {
        Result<PaginationDto<ProductShortDto>> getProductsByCategoryResult =
            await productService.GetForPageByCategoryIdAsync(childCategoryId, page, pageSize, ct);

        if (!getProductsByCategoryResult.IsSuccess)
        {
            return TypedResults.NotFound(getProductsByCategoryResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getProductsByCategoryResult.Value);
    }

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, NotFound<string>>> GetProductsWithDiscountForPage(
        [FromRoute] int page,
        [FromQuery] int pageSize,
        CancellationToken ct,
        [FromServices] IProductService productService)
    {
        Result<PaginationDto<ProductShortDto>> getProductsWithDiscountResult =
            await productService.GetForPageWithDiscountAsync(page, pageSize, ct);

        if (!getProductsWithDiscountResult.IsSuccess)
        {
            return TypedResults.NotFound(getProductsWithDiscountResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getProductsWithDiscountResult.Value);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<ProductShortDto>>, BadRequest<string>, NotFound<string>>>
        GetRecommendedProductsForPage(
            [FromQuery] int pageSize,
            [FromQuery] Guid[] visitedCategoriesIds,
            CancellationToken ct,
            [FromServices] IProductService productService)
    {
        if (pageSize < 1)
        {
            return TypedResults.BadRequest("Page size must be greater than 0");
        }

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

    private static async Task<Results<Ok<IReadOnlyCollection<ProductShortDto>>, NotFound<string>>> GetRandomProducts(
        [FromQuery] int pageSize,
        CancellationToken ct,
        [FromServices] IProductService productService)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getRandomProductsResult =
            await productService.GetRandomProductsAsync(pageSize, ct);

        if (!getRandomProductsResult.IsSuccess)
        {
            return TypedResults.NotFound(getRandomProductsResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getRandomProductsResult.Value);
    }
}