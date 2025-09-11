using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        
        var changeGroup = sellerGroup.MapGroup("{sellerId:guid}");
        MapChangeEndpoints(changeGroup);
        
        //todo: ендпоинт для смены фото
    }
    
    private static void MapChangeEndpoints(RouteGroupBuilder group)
    {
        group.MapPatch("full-name", ChangeFullName)
            .WithSummary("Change full name");
        
        group.MapPatch("phone-number", ChangePhoneNumber)
            .WithSummary("Change phone number");
        
        group.MapPatch("address", ChangeAddress)
            .WithSummary("Change address");
        
        group.MapPatch("birth-date", ChangeBirthDate)
            .WithSummary("Change birth date");
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
                _ => throw new Exception("Unknown status code")
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
                _ => throw new Exception("Unknown status code")
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
                _ => throw new Exception("Unknown status code")
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
                _ => throw new Exception("Unknown status code")
            };
        }
        
        return TypedResults.Ok(changeBirthDateResult.Value);
    }
}