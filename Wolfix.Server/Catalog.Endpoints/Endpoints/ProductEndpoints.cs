using System.Net;
using Catalog.Application.Dto.Discount.Requests;
using Catalog.Application.Dto.Product;
using Catalog.Application.Dto.Product.AdditionDtos;
using Catalog.Application.Dto.Product.AttributesFiltrationDto;
using Catalog.Application.Dto.Product.Change;
using Catalog.Application.Dto.Product.FullDto;
using Catalog.Application.Dto.Product.Review;
using Catalog.Application.Services;
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
            .RequireAuthorization("Seller")
            .WithSummary("Add product");

        group.MapPatch("product/{productId:guid}/new-main-photo/{newMainPhotoId:guid}", ChangeProductMainPhoto)
            .RequireAuthorization("Seller")
            .WithSummary("Change product main photo");
        
        group.MapPatch("product/{productId:guid}/general-info", ChangeProductGeneralInfo)
            .RequireAuthorization("Seller")
            .WithSummary("Change product general info");
        
        group.MapPatch("product/{productId:guid}/price", ChangePrice)
            .RequireAuthorization("Seller")
            .WithSummary("Change product price");

        group.MapPatch("add-product-media", AddProductMedia)
            .DisableAntiforgery()
            .RequireAuthorization("Seller")
            .WithSummary("Add product media");

        group.MapDelete("product/{productId:guid}/media/{mediaId:guid}", DeleteProductMedia)
            .RequireAuthorization("Seller")
            .WithSummary("Delete product media");
        
        group.MapGet("product/{productId:guid}", GetProductFullInfo)
            .WithSummary("Get product full info");

        group.MapGet("category/{childCategoryId:guid}/page/{page:int}", GetAllByCategoryForPage)
            .WithSummary("Get all products by specific category for page with pagination");

        group.MapGet("with-discount/page/{page:int}", GetWithDiscountForPage)
            .WithSummary("Get all products with discount for page with pagination");

        group.MapGet("recommended", GetRecommendedForPage)
            .WithSummary("Get recommended products by visitedCategories list for page with pagination");

        group.MapGet("random", GetRandom)
            .WithSummary("Get random products");
        
        group.MapGet("seller/{sellerId:guid}/category/{categoryId:guid}/page/{page:int}", GetAllBySellerCategoryForPage)
            .RequireAuthorization("Seller")
            .WithSummary("Get all products by specific seller category");
        
        group.MapGet("search/category/{categoryId:guid}", GetSearchByCategory)
            .WithSummary("Get products by search query and category");
        
        group.MapGet("search", GetSearch)
            .WithSummary("Get products by search query");
        
        group.MapPost("filter-by-attributes", GetProductsByAttributes)
            .WithSummary("Get products by attributes filtration");
        
        group.MapPost("product/{productId:guid}/discount", AddDiscount)
            .RequireAuthorization("Seller")
            .WithSummary("Add discount to product");

        group.MapDelete("product/{productId:guid}/discount", DeleteDiscount)
            .RequireAuthorization("Seller")
            .WithSummary("Delete product discount");
    }

    private static void MapReviewEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("", GetReviews)
            .WithSummary("Get all reviews by specific product");
        
        group.MapPost("", AddReview)
            .RequireAuthorization("Customer")
            .WithSummary("Add review");
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> AddProduct(
        [FromForm] AddProductDto addProductDto,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult addProductResult = await productService.AddProductAsync(addProductDto, ct);

        if (addProductResult.IsFailure)
        {
            return addProductResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addProductResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(addProductResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(AddProduct)} -> Unknown status code: {addProductResult.StatusCode}")
            };
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> AddDiscount(
        [FromRoute] Guid productId,
        [FromBody] AddDiscountDto request,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult addDiscountResult = await productService.AddDiscountAsync(productId, request, ct);

        if (addDiscountResult.IsFailure)
        {
            return addDiscountResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(addDiscountResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addDiscountResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(AddDiscount)} -> Unknown status code: {addDiscountResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> DeleteDiscount(
        [FromRoute] Guid productId,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult deleteDiscountResult = await productService.DeleteDiscountAsync(productId, ct);

        if (deleteDiscountResult.IsFailure)
        {
            return deleteDiscountResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(deleteDiscountResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteDiscountResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(DeleteDiscount)} -> Unknown status code: {deleteDiscountResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> ChangeProductMainPhoto(
        [FromRoute] Guid productId,
        [FromRoute] Guid newMainPhotoId,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult changeMainPhotoResult =
            await productService.ChangeProductMainPhotoAsync(productId, newMainPhotoId, ct);

        if (changeMainPhotoResult.IsFailure)
        {
            return changeMainPhotoResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeMainPhotoResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(changeMainPhotoResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeProductMainPhoto)} -> Unknown status code: {changeMainPhotoResult.StatusCode}")
            };
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> ChangeProductGeneralInfo(
        [FromRoute] Guid productId,
        [FromBody] ChangeProductGeneralInfoDto request,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult changeProductGeneralInfoResult = await productService.ChangeProductGeneralInfoAsync(productId, request, ct);

        if (changeProductGeneralInfoResult.IsFailure)
        {
            return changeProductGeneralInfoResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeProductGeneralInfoResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeProductGeneralInfoResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeProductGeneralInfo)} -> Unknown status code: {changeProductGeneralInfoResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> ChangePrice(
        [FromRoute] Guid productId,
        [FromBody] ChangeProductPriceDto request,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult changeProductResult = await productService.ChangeProductPrice(productId, request, ct);

        if (changeProductResult.IsFailure)
        {
            return changeProductResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeProductResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeProductResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangePrice)} -> Unknown status code: {changeProductResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> AddProductMedia(
        [FromForm] AddMediaDto addMediaDto,
        [FromServices] ProductService productService,
        CancellationToken ct
    )
    {
        VoidResult addProductMediaResult = await productService.AddProductMediaAsync(addMediaDto, ct);
        
        if (addProductMediaResult.IsFailure)
        {
            return addProductMediaResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addProductMediaResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(addProductMediaResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(AddProductMedia)} -> Unknown status code: {addProductMediaResult.StatusCode}")
            };
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> DeleteProductMedia(
        [FromRoute] Guid productId,
        [FromRoute] Guid mediaId,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult deleteProductMediaResult = await productService.DeleteProductMediaAsync(productId, mediaId, ct);

        if (deleteProductMediaResult.IsFailure)
        {
            return deleteProductMediaResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteProductMediaResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(deleteProductMediaResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(DeleteProductMedia)} -> Unknown status code: {deleteProductMediaResult.StatusCode}")
            };
        }

        return TypedResults.NoContent();
    }
    
    private static async Task<Results<Ok<ProductFullDto>, BadRequest<string>, NotFound<string>>> GetProductFullInfo(
        [FromRoute] Guid productId,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        Result<ProductFullDto> getProductFullInfoResult =
            await productService.GetProductFullInfoAsync(productId, ct);

        if (getProductFullInfoResult.IsFailure)
        {
            return getProductFullInfoResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(getProductFullInfoResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(getProductFullInfoResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(GetProductFullInfo)} -> Unknown status code: {getProductFullInfoResult.StatusCode}")
            };
        }

        return TypedResults.Ok(getProductFullInfoResult.Value);
    }

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, BadRequest<string>, NotFound<string>>>
        GetAllByCategoryForPage(
            [FromRoute] Guid childCategoryId,
            [FromRoute] int page,
            [FromServices] ProductService productService,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
    {
        if (page < 1)
        {
            return TypedResults.BadRequest("Page must be greater than 0");
        }

        Result<PaginationDto<ProductShortDto>> getProductsByCategoryResult =
            await productService.GetForPageByCategoryIdAsync(childCategoryId, page, pageSize, ct);

        if (getProductsByCategoryResult.IsFailure)
        {
            return TypedResults.NotFound(getProductsByCategoryResult.ErrorMessage);
        }

        return TypedResults.Ok(getProductsByCategoryResult.Value);
    }

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, BadRequest<string>>> GetWithDiscountForPage(
        [FromRoute] int page,
        [FromServices] ProductService productService,
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
            [FromServices] ProductService productService,
            CancellationToken ct,
            [FromQuery] int pageSize = 12)
    {
        if (visitedCategoriesIds.Length == 0)
        {
            return TypedResults.BadRequest("Visited categories must be not empty");
        }

        Result<IReadOnlyCollection<ProductShortDto>> getRecommendedProductsResult =
            await productService.GetRecommendedForPageAsync(pageSize, visitedCategoriesIds.ToList(), ct);

        if (getRecommendedProductsResult.IsFailure)
        {
            return TypedResults.NotFound(getRecommendedProductsResult.ErrorMessage);
        }

        return TypedResults.Ok(getRecommendedProductsResult.Value);
    }

    private static async Task<Ok<IReadOnlyCollection<ProductShortDto>>> GetRandom(
        [FromServices] ProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 12)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getRandomProductsResult =
            await productService.GetRandomProductsAsync(pageSize, ct);

        return TypedResults.Ok(getRandomProductsResult.Value);
    }

    private static async Task<Results<Ok<PaginationDto<ProductShortDto>>, NotFound<string>>> GetAllBySellerCategoryForPage(
        [FromRoute] Guid sellerId,
        [FromRoute] Guid categoryId,
        [FromRoute] int page,
        [FromServices] ProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 20)
    {
        Result<PaginationDto<ProductShortDto>> getProductsBySellerCategoryResult =
            await productService.GetAllBySellerCategoryForPageAsync(sellerId, categoryId, page, pageSize, ct);

        if (getProductsBySellerCategoryResult.IsFailure)
        {
            return TypedResults.NotFound(getProductsBySellerCategoryResult.ErrorMessage);
        }

        return TypedResults.Ok(getProductsBySellerCategoryResult.Value);
    }

    private static async Task<Results<Ok<CursorPaginationDto<ProductReviewDto>>, NotFound<string>>> GetReviews(
        [FromRoute] Guid productId,
        [FromServices] ProductService productService,
        CancellationToken ct,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? lastId = null)
    {
        Result<CursorPaginationDto<ProductReviewDto>> getProductReviewsResult =
            await productService.GetReviewsAsync(productId, pageSize, lastId, ct);

        if (getProductReviewsResult.IsFailure)
        {
            return TypedResults.NotFound(getProductReviewsResult.ErrorMessage);
        }

        return TypedResults.Ok(getProductReviewsResult.Value);
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> AddReview(
        [FromBody] AddProductReviewDto addProductReviewDto,
        [FromRoute] Guid productId,
        [FromServices] ProductService productService,
        CancellationToken ct)
    {
        VoidResult addReviewResult = await productService.AddReviewAsync(productId, addProductReviewDto, ct);

        if (addReviewResult.IsFailure)
        {
            return addReviewResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(addReviewResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addReviewResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(AddReview)} -> Unknown status code: {addReviewResult.StatusCode}")
            };
        }

        return TypedResults.NoContent();
    }
    
    private static async Task<Results<Ok<IReadOnlyCollection<ProductShortDto>>, BadRequest<string>, NotFound<string>>>
        GetSearchByCategory(
            [FromRoute] Guid categoryId,
            [FromQuery] string searchQuery,
            [FromServices] ProductService productService,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getProductsBySearchQueryAndCategoryResult =
            await productService.GetBySearchQueryAndCategoryAsync(categoryId, searchQuery, pageSize, ct);

        if (getProductsBySearchQueryAndCategoryResult.IsFailure)
        {
            return getProductsBySearchQueryAndCategoryResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(getProductsBySearchQueryAndCategoryResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(getProductsBySearchQueryAndCategoryResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeProductMainPhoto)} -> Unknown status code: {getProductsBySearchQueryAndCategoryResult.StatusCode}")
            };
        }

        return TypedResults.Ok(getProductsBySearchQueryAndCategoryResult.Value);
    }
    
    private static async Task<Results<Ok<IReadOnlyCollection<ProductShortDto>>, BadRequest<string>>>
        GetSearch(
            [FromQuery] string searchQuery,
            [FromServices] ProductService productService,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getProductsBySearchQueryAndCategoryResult =
            await productService.GetBySearchQueryAsync(searchQuery, pageSize, ct);

        if (getProductsBySearchQueryAndCategoryResult.IsFailure)
        {
            return getProductsBySearchQueryAndCategoryResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(getProductsBySearchQueryAndCategoryResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeProductMainPhoto)} -> Unknown status code: {getProductsBySearchQueryAndCategoryResult.StatusCode}")
            };
        }

        return TypedResults.Ok(getProductsBySearchQueryAndCategoryResult.Value);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<ProductShortDto>>, BadRequest<string>, NotFound<string>>>
        GetProductsByAttributes(
            [FromBody] AttributesFiltrationDto attributesFiltrationDto,
            [FromServices] ProductService productService,
            CancellationToken ct,
            [FromQuery] int pageSize = 20)
    {
        Result<IReadOnlyCollection<ProductShortDto>> getProductsByAttributes =
            await productService.GetByAttributesFiltrationAsync(attributesFiltrationDto, pageSize, ct);

        if (getProductsByAttributes.IsFailure)
        {
            return getProductsByAttributes.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(getProductsByAttributes.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(getProductsByAttributes.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeProductMainPhoto)} -> Unknown status code: {getProductsByAttributes.StatusCode}")
            };
        }

        return TypedResults.Ok(getProductsByAttributes.Value);
    }
}