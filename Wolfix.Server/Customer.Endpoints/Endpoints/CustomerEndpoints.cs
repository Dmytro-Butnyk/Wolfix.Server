using System.Net;
using Customer.Application.Dto.CartItem;
using Customer.Application.Dto.Customer;
using Customer.Application.Dto.FavoriteItem;
using Customer.Application.Dto.Product;
using Customer.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Application.Dto;
using Shared.Domain.Models;
using Shared.Endpoints;
using Shared.Endpoints.Exceptions;

namespace Customer.Endpoints.Endpoints;

internal static class CustomerEndpoints
{
    private const string Route = "api/customers";

    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customerGroup = app.MapGroup(Route)
            .WithTags("Customer");
        
        customerGroup.MapGet("{customerId:guid}", GetProfileInfo)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Get profile info");
        
        var favoriteItemsGroup = customerGroup.MapGroup("favorites");
        MapFavoriteItemsEndpoints(favoriteItemsGroup);
        
        var cartItemsGroup = customerGroup.MapGroup("cart-items");
        MapCartItemsEndpoints(cartItemsGroup);
        
        var changeMethodsGroup = customerGroup.MapGroup("{customerId:guid}");
        MapChangeMethods(changeMethodsGroup);
        
        //todo: ендпоинт для смены фото
    }
    
    private static void MapFavoriteItemsEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("{customerId:guid}", GetFavoriteProducts)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Get all favorite products by specific customer");
        
        group.MapPost("", AddProductToFavorite)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Add product to favorite");
        
        group.MapDelete("{customerId:guid}/{favoriteItemId:guid}", DeleteFavoriteItem)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Delete favorite item");
    }

    private static void MapCartItemsEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("{customerId:guid}", GetCartProducts)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Get all products in cart by specific customer");
        
        group.MapPost("", AddProductToCart)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Add product to cart");
        
        group.MapDelete("{customerId:guid}/{cartItemId:guid}", DeleteCartItem)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Delete cart item");
    }

    private static void MapChangeMethods(RouteGroupBuilder group)
    {
        group.MapPatch("full-name", ChangeFullName)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Change full name");

        group.MapPatch("phone-number", ChangePhoneNumber)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Change phone number");
        
        group.MapPatch("address", ChangeAddress)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Change address");
        
        group.MapPatch("birth-date", ChangeBirthDate)
            .RequireAuthorization(AuthorizationRoles.Customer)
            .WithSummary("Change birth date");
    }
    
    private static async Task<Results<Ok<CustomerDto>, NotFound<string>>> GetProfileInfo(
        [FromRoute] Guid customerId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        Result<CustomerDto> getProfileInfoResult = await customerService.GetProfileInfoAsync(customerId, ct);

        if (getProfileInfoResult.IsFailure)
        {
            return TypedResults.NotFound(getProfileInfoResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getProfileInfoResult.Value);
    }
    
    private static async Task<Results<Ok<IReadOnlyCollection<FavoriteItemDto>>, NotFound<string>>> GetFavoriteProducts(
        [FromRoute] Guid customerId,
        [FromServices] CustomerService customerService,
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
        [FromServices] CustomerService customerService,
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
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        VoidResult addProductToFavoriteResult = await customerService.AddProductToFavoriteAsync(request, ct);

        if (!addProductToFavoriteResult.IsSuccess)
        {
            return TypedResults.NotFound(addProductToFavoriteResult.ErrorMessage);
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>>> DeleteFavoriteItem(
        [FromRoute] Guid customerId,
        [FromRoute] Guid favoriteItemId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        VoidResult deleteFavoriteItemResult = await customerService.DeleteFavoriteItemAsync(customerId, favoriteItemId, ct);

        if (deleteFavoriteItemResult.IsFailure)
        {
            return TypedResults.NotFound(deleteFavoriteItemResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>>> AddProductToCart(
        [FromBody] AddProductToCartDto request,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        VoidResult addProductToCartResult = await customerService.AddProductToCartAsync(request, ct);

        if (!addProductToCartResult.IsSuccess)
        {
            return TypedResults.NotFound(addProductToCartResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>>> DeleteCartItem(
        [FromRoute] Guid customerId,
        [FromRoute] Guid cartItemId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        VoidResult deleteCartItemResult = await customerService.DeleteCartItemAsync(customerId, cartItemId, ct);

        if (deleteCartItemResult.IsFailure)
        {
            return TypedResults.NotFound(deleteCartItemResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<FullNameDto>, NotFound<string>, BadRequest<string>>> ChangeFullName(
        [FromBody] ChangeFullNameDto request,
        [FromRoute] Guid customerId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        Result<FullNameDto> changeFullNameResult = await customerService.ChangeFullNameAsync(customerId, request, ct);
        
        if (!changeFullNameResult.IsSuccess)
        {
            return changeFullNameResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeFullNameResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeFullNameResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(ChangeFullName), changeFullNameResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(changeFullNameResult.Value);
    }

    private static async Task<Results<Ok<string>, NotFound<string>, BadRequest<string>>> ChangePhoneNumber(
        [FromBody] ChangePhoneNumberDto request,
        [FromRoute] Guid customerId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        Result<string> changePhoneNumberResult = await customerService.ChangePhoneNumberAsync(customerId, request, ct);

        if (!changePhoneNumberResult.IsSuccess)
        {
            return changePhoneNumberResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changePhoneNumberResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changePhoneNumberResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(ChangePhoneNumber), changePhoneNumberResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(changePhoneNumberResult.Value);
    }

    private static async Task<Results<Ok<AddressDto>, NotFound<string>, BadRequest<string>>> ChangeAddress(
        [FromBody] ChangeAddressDto request,
        [FromRoute] Guid customerId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        Result<AddressDto> changeAddressResult = await customerService.ChangeAddressAsync(customerId, request, ct);
        
        if (!changeAddressResult.IsSuccess)
        {
            return changeAddressResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeAddressResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeAddressResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(ChangeAddress), changeAddressResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(changeAddressResult.Value);
    }

    private static async Task<Results<Ok<string>, NotFound<string>, BadRequest<string>>> ChangeBirthDate(
        [FromBody] ChangeBirthDateDto request,
        [FromRoute] Guid customerId,
        [FromServices] CustomerService customerService,
        CancellationToken ct)
    {
        Result<string> changeBirthDateResult = await customerService.ChangeBirthDateAsync(customerId, request, ct);
        
        if (!changeBirthDateResult.IsSuccess)
        {
            return changeBirthDateResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeBirthDateResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeBirthDateResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(ChangeBirthDate), changeBirthDateResult.StatusCode)
            };
        }
        
        return TypedResults.Ok(changeBirthDateResult.Value);
    }
}