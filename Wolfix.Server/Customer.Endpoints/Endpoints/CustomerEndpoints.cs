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
    }

    private static async Task<Results<Ok, BadRequest<string>>> AddProductToFavorite(
        [FromBody] AddProductToFavoriteDto request,
        [FromServices] ICustomerService customerService,
        CancellationToken ct)
    {
        VoidResult addProductToFavoriteResult = await customerService.AddProductToFavoriteAsync(request, ct);

        if (!addProductToFavoriteResult.IsSuccess)
        {
            
        }

        return TypedResults.Ok();
    }
}