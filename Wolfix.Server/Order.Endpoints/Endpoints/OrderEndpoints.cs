using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Order.Application.Dto.Order.Requests;
using Order.Application.Interfaces;
using Shared.Domain.Models;

namespace Order.Endpoints.Endpoints;

internal static class OrderEndpoints
{
    private const string Route = "api/orders";

    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orderGroup = app.MapGroup(Route)
            .WithTags("Order");
        
        orderGroup.MapPost("", PlaceOrder)
            .WithSummary("Creates an order and returns client secret for payment");
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> PlaceOrder(
        [FromBody] PlaceOrderDto request,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        Result<string> placeOrderResult = await orderService.PlaceOrderAsync(request, ct);

        if (placeOrderResult.IsFailure)
        {
            return TypedResults.BadRequest(placeOrderResult.ErrorMessage);
        }
        
        return TypedResults.Ok(placeOrderResult.Value!);
    }
}