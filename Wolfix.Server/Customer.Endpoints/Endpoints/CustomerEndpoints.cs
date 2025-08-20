using System.Net;
using Customer.Application.Dto;
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
        
        customerGroup.MapPost("add-product-to-favorite", AddProductToFavorite);
    }
    
    private static async Task<Results<Ok, NotFound<string>, Conflict<string>, BadRequest<string>>> AddProductToFavorite(
        [FromBody] AddProductToFavoriteDto request,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        VoidResult addProductToFavoriteResult = await customerService.AddProductToFavoriteAsync(request, ct);

        if (!addProductToFavoriteResult.IsSuccess)
        {
            return addProductToFavoriteResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(addProductToFavoriteResult.ErrorMessage),
                HttpStatusCode.Conflict => TypedResults.Conflict(addProductToFavoriteResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(addProductToFavoriteResult.ErrorMessage),
                _ => throw new Exception("Unknown status code")
            };
        }

        return TypedResults.Ok();
    }
}