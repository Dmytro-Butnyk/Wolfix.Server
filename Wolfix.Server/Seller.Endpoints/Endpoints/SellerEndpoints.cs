using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Seller.Application.Dto.Seller;
using Seller.Application.Interfaces;
using Shared.Application.Dto;
using Shared.Domain.Models;

namespace Seller.Endpoints.Endpoints;

internal static class SellerEndpoints
{
    private const string Route = "api/sellers";
    
    public static void MapSellerEndpoints(this IEndpointRouteBuilder app)
    {
        var sellerGroup = app.MapGroup(Route)
            .WithTags("Seller");

        sellerGroup.MapGet("{sellerId:guid}", GetProfileInfo)
            .RequireAuthorization("Seller")
            .WithSummary("Get profile info");
        
        var changeGroup = sellerGroup.MapGroup("{sellerId:guid}");
        MapChangeEndpoints(changeGroup);

        changeGroup.MapGet("categories", GetAllSellerCategories)
            .RequireAuthorization("Seller")
            .WithSummary("Get all seller categories");

        //todo: ендпоинт для смены фото
    }
    
    private static void MapChangeEndpoints(RouteGroupBuilder group)
    {
        group.MapPatch("full-name", ChangeFullName)
            .RequireAuthorization("Seller")
            .WithSummary("Change full name");
        
        group.MapPatch("phone-number", ChangePhoneNumber)
            .RequireAuthorization("Seller")
            .WithSummary("Change phone number");
        
        group.MapPatch("address", ChangeAddress)
            .RequireAuthorization("Seller")
            .WithSummary("Change address");
        
        group.MapPatch("birth-date", ChangeBirthDate)
            .RequireAuthorization("Seller")
            .WithSummary("Change birth date");
    }

    private static async Task<Results<Ok<SellerDto>, NotFound<string>>> GetProfileInfo(
        [FromRoute] Guid sellerId,
        [FromServices] ISellerService sellerService,
        CancellationToken ct)
    {
        Result<SellerDto> getProfileInfoResult = await sellerService.GetProfileInfoAsync(sellerId, ct);

        if (getProfileInfoResult.IsFailure)
        {
            return TypedResults.NotFound(getProfileInfoResult.ErrorMessage);
        }
        
        return TypedResults.Ok(getProfileInfoResult.Value);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<SellerCategoryDto>>, NotFound<string>>> GetAllSellerCategories(
        [FromRoute] Guid sellerId,
        [FromServices] ISellerService sellerService,
        CancellationToken ct)
    {
        Result<IReadOnlyCollection<SellerCategoryDto>> getSellerCategories = await sellerService.GetAllSellerCategoriesAsync(sellerId, ct);

        if (getSellerCategories.IsFailure)
        {
            return TypedResults.NotFound(getSellerCategories.ErrorMessage);
        }

        return TypedResults.Ok(getSellerCategories.Value!);
    }

    private static async Task<Results<Ok<FullNameDto>, NotFound<string>, BadRequest<string>>> ChangeFullName(
        [FromBody] ChangeFullNameDto request,
        [FromRoute] Guid sellerId,
        [FromServices] ISellerService sellerService,
        CancellationToken ct)
    {
        Result<FullNameDto> changeFullNameResult = await sellerService.ChangeFullNameAsync(sellerId, request, ct);

        if (!changeFullNameResult.IsSuccess)
        {
            return changeFullNameResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeFullNameResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeFullNameResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeFullName)} -> Unknown status code: {changeFullNameResult.StatusCode}")
            };
        }
        
        return TypedResults.Ok(changeFullNameResult.Value);
    }

    private static async Task<Results<Ok<string>, NotFound<string>, BadRequest<string>>> ChangePhoneNumber(
        [FromBody] ChangePhoneNumberDto request,
        [FromRoute] Guid sellerId,
        [FromServices] ISellerService sellerService,
        CancellationToken ct)
    {
        Result<string> changePhoneNumberResult = await sellerService.ChangePhoneNumberAsync(sellerId, request, ct);

        if (!changePhoneNumberResult.IsSuccess)
        {
            return changePhoneNumberResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changePhoneNumberResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changePhoneNumberResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangePhoneNumber)} -> Unknown status code: {changePhoneNumberResult.StatusCode}")
            };
        }
        
        return TypedResults.Ok(changePhoneNumberResult.Value);
    }

    private static async Task<Results<Ok<AddressDto>, NotFound<string>, BadRequest<string>>> ChangeAddress(
        [FromBody] ChangeAddressDto request,
        [FromRoute] Guid sellerId,
        [FromServices] ISellerService sellerService,
        CancellationToken ct)
    {
        Result<AddressDto> changeAddressResult = await sellerService.ChangeAddressAsync(sellerId, request, ct);

        if (!changeAddressResult.IsSuccess)
        {
            return changeAddressResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeAddressResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeAddressResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeAddress)} -> Unknown status code: {changeAddressResult.StatusCode}")
            };
        }
        
        return TypedResults.Ok(changeAddressResult.Value);
    }

    private static async Task<Results<Ok<string>, NotFound<string>, BadRequest<string>>> ChangeBirthDate(
        [FromBody] ChangeBirthDateDto request,
        [FromRoute] Guid sellerId,
        [FromServices] ISellerService sellerService,
        CancellationToken ct)
    {
        Result<string> changeBirthDateResult = await sellerService.ChangeBirthDateAsync(sellerId, request, ct);

        if (!changeBirthDateResult.IsSuccess)
        {
            return changeBirthDateResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(changeBirthDateResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(changeBirthDateResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(ChangeBirthDate)} -> Unknown status code: {changeBirthDateResult.StatusCode}")
            };
        }
        
        return TypedResults.Ok(changeBirthDateResult.Value);
    }
}