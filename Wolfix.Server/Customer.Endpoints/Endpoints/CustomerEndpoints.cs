using System.Net;
using Customer.Application.Dto;
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
    }
    
    private static void MapFavoriteItemsEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("{customerId:guid}", GetFavoriteItems);
        group.MapPost("", AddProductToFavorite);
    }
    
    //todo: доделать все ендпоинты

    private static async Task<Results<Ok<IReadOnlyCollection<FavoriteItemDto>>, NotFound<string>>> GetFavoriteItems(
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
}