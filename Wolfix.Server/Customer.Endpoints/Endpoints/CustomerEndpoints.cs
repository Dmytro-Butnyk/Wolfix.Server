using System.Net;
using Customer.Application.Dto;
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
        group.MapGet("{customerId:guid}", GetFavoriteProducts);
        group.MapPost("", AddProductToFavorite);
    }

    private static void MapCartItemsEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("{customerId:guid}", GetCartProducts);
        group.MapPost("", AddProductToCart);
    }
    
    //todo: доделать все ендпоинты

    //todo: подумать надо ли тут пагинация
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

    //todo: подумать надо ли тут пагинация
    private static async Task<Results<Ok<IReadOnlyCollection<CartItemDto>>, NotFound<string>>> GetCartProducts(
        [FromRoute] Guid customerId,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        Result<IReadOnlyCollection<CartItemDto>> getCartItemsResult =
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