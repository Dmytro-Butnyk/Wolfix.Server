using Customer.Application.Dto.CartItem;
using Customer.Application.Dto.FavoriteItem;
using Customer.Application.Dto.Product;
using Customer.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;

namespace Customer.Endpoints.Endpoints;

internal static class CustomerEndpoints
{
    private const string Route = "api/customer";

    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customerGroup = app.MapGroup(Route)
            .WithTags("Customer");
        
        var favoriteItemsGroup = customerGroup.MapGroup("favorites");
        MapFavoriteItemsEndpoints(favoriteItemsGroup);
        
        var cartItemsGroup = customerGroup.MapGroup("cart-items");
        MapCartItemsEndpoints(cartItemsGroup);
    }
    
    private static void MapFavoriteItemsEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("{customerId:guid}", GetFavoriteProducts)
            .WithSummary("Get all favorite products by specific customer");
        
        group.MapPost("", AddProductToFavorite)
            .WithSummary("Add product to favorite");
    }

    private static void MapCartItemsEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("{customerId:guid}", GetCartProducts)
            .WithSummary("Get all products in cart by specific customer");
        
        group.MapPost("", AddProductToCart)
            .WithSummary("Add product to cart");
    }
    
    //todo: эндпоинт для того чтобы отзыв оставить
    
    private static async Task<Results<Ok<IReadOnlyCollection<FavoriteItemDto>>, NotFound<string>>> GetFavoriteProducts(
        [FromRoute] Guid customerId,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        Result<IReadOnlyCollection<FavoriteItemDto>> getFavoriteItemsResult =
            await customerService.GetFavoriteItemsAsync(customerId, ct);
        
        if (!getFavoriteItemsResult.IsSuccess)
        {
            return TypedResults.NotFound(getFavoriteItemsResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getFavoriteItemsResult.Value);
    }

    private static async Task<Results<Ok<CustomerCartItemsDto>, NotFound<string>>> GetCartProducts(
        [FromRoute] Guid customerId,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        Result<CustomerCartItemsDto> getCartItemsResult =
            await customerService.GetCartItemsAsync(customerId, ct);
        
        if (!getCartItemsResult.IsSuccess)
        {
            return TypedResults.NotFound(getCartItemsResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getCartItemsResult.Value);
    }
    
    private static async Task<Results<NoContent, NotFound<string>>> AddProductToFavorite(
        [FromBody] AddProductToFavoriteDto request,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        VoidResult addProductToFavoriteResult = await customerService.AddProductToFavoriteAsync(request, ct);

        if (!addProductToFavoriteResult.IsSuccess)
        {
            return TypedResults.NotFound(addProductToFavoriteResult.ErrorMessage);
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>>> AddProductToCart(
        [FromBody] AddProductToCartDto request,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        VoidResult addProductToCartResult = await customerService.AddProductToCartAsync(request, ct);

        if (!addProductToCartResult.IsSuccess)
        {
            return TypedResults.NotFound(addProductToCartResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }
}