using System.Net;
using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.AdditionDtos;
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

        MapProductEndpoints(productGroup);

        var reviewGroup = productGroup.MapGroup("{productId:guid}/reviews");
        MapReviewEndpoints(reviewGroup);
    }

    private static void MapProductEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("", AddProduct)
            .DisableAntiforgery()
            .WithSummary("Add product");

        group.MapPatch("product/{productId:guid}/new-main-photo/{newMainPhotoId:guid}", ChangeProductMainPhoto)
            .WithSummary("Change product main photo");

        group.MapPatch("add-product-media", AddProductMedia)
            .DisableAntiforgery()
            .WithSummary("Add product media");

        group.MapDelete("product/{productId:guid}/media/{mediaId:guid}", DeleteProductMedia)
            .WithSummary("Delete product media");

        group.MapGet("category/{childCategoryId:guid}/page/{page:int}", GetAllByCategoryForPage)
            .WithSummary("Get all products by specific category for page with pagination");

        group.MapGet("with-discount/page/{page:int}", GetWithDiscountForPage)
            .WithSummary("Get all products with discount for page with pagination");

        group.MapGet("recommended", GetRecommendedForPage)
            .WithSummary("Get recommended products by visitedCategories list for page with pagination");

        group.MapGet("random", GetRandom)
            .WithSummary("Get random products");
    }

    private static void MapReviewEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("", GetReviews)
            .WithSummary("Get all reviews by specific product");
        //todo: протестить
        group.MapPost("", AddReview)
            .WithSummary("Add review");
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> AddProduct(
        [FromForm] AddProductDto addProductDto,
        [FromServices] IProductService productService,
        CancellationToken ct)
    {
        VoidResult addProductResult = await productService.AddProductAsync(addProductDto, ct);

        if (!addProductResult.IsSuccess)
        {
            return addProductResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addProductResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(addProductResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> ChangeProductMainPhoto(
        [FromRoute] Guid productId,
        [FromRoute] Guid newMainPhotoId,
        [FromServices] IProductService productService,
        CancellationToken ct)
    {
        VoidResult changeMainPhotoResult =
            await productService.ChangeProductMainPhotoAsync(productId, newMainPhotoId, ct);

        if (!changeMainPhotoResult.IsSuccess)
        {
            return changeMainPhotoResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeMainPhotoResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(changeMainPhotoResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> AddProductMedia(
        [FromForm] AddMediaDto addMediaDto,
        [FromServices] IProductService productService,
        CancellationToken ct
    )
    {
        VoidResult addProductMediaResult = await productService.AddProductMediaAsync(addMediaDto, ct);
        
        if (!addProductMediaResult.IsSuccess)
        {
            return addProductMediaResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addProductMediaResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(addProductMediaResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> DeleteProductMedia(
        [FromRoute] Guid productId,
        [FromRoute] Guid mediaId,
        [FromServices] IProductService productService,
        CancellationToken ct)
    {
        VoidResult deleteProductMediaResult = await productService.DeleteProductMediaAsync(productId, mediaId, ct);

        if (!deleteProductMediaResult.IsSuccess)
        {
            return deleteProductMediaResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteProductMediaResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(deleteProductMediaResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }

        return TypedResults.NoContent();
    }

    //TODO: ПРОВЕРИТЬ СТАТУС КОДЫ
    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, BadRequest<string>, NotFound<string>>>
        GetAllByCategoryForPage(
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

    //todo: что-то не так тут
    private static async Task<Ok<IReadOnlyCollection<ProductShortDto>>> GetRandom(
        [FromServices] IProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 12)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getRandomProductsResult =
            await productService.GetRandomProductsAsync(pageSize, ct);

        return TypedResults.Ok(getRandomProductsResult.Value);
    }

    private static async Task<Results<Ok<CursorPaginationDto<ProductReviewDto>>, NotFound<string>>> GetReviews(
        [FromRoute] Guid productId,
        [FromServices] IProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? lastId = null)
    {
        Result<CursorPaginationDto<ProductReviewDto>> getProductReviewsResult =
            await productService.GetReviewsAsync(productId, pageSize, lastId, ct);

        if (!getProductReviewsResult.IsSuccess)
        {
            return TypedResults.NotFound(getProductReviewsResult.ErrorMessage);
        }

        return TypedResults.Ok(getProductReviewsResult.Value);
    }

    private static async Task<Results<NoContent, NotFound<string>>> AddReview(
        [FromBody] AddProductReview addProductReviewDto,
        [FromRoute] Guid productId,
        [FromServices] IProductService productService,
        CancellationToken ct)
    {
        VoidResult addReviewResult = await productService.AddReviewAsync(productId, addProductReviewDto, ct);

        if (!addReviewResult.IsSuccess)
        {
            return TypedResults.NotFound(addReviewResult.ErrorMessage);
        }

        return TypedResults.NoContent();
    }
}