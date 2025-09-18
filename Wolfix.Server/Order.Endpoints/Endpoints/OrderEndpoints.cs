using System.Net;
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
        
        orderGroup.MapPost("with-payment", PlaceOrderWithPayment)
            .WithSummary("Creates an order and returns client secret for payment");

        orderGroup.MapPatch("{orderId:guid}/paid", MarkOrderPaid)
            .WithSummary("Marks order as paid");

        orderGroup.MapPost("", PlaceOrder)
            .WithSummary("Creates an order without payment");
    }

    private static async Task<Results<Ok<string>, BadRequest<string>, NotFound<string>, InternalServerError<string>>> PlaceOrderWithPayment(
        [FromBody] PlaceOrderDto request,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        Result<string> placeOrderWithPaymentResult = await orderService.PlaceOrderWithPaymentAsync(request, ct);

        if (placeOrderWithPaymentResult.IsFailure)
        {
            return placeOrderWithPaymentResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(placeOrderWithPaymentResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(placeOrderWithPaymentResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(placeOrderWithPaymentResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(PlaceOrderWithPayment)} -> Unknown status code: {placeOrderWithPaymentResult.StatusCode}")
            };
        }
        
        return TypedResults.Ok(placeOrderWithPaymentResult.Value!);
    }

    private static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> PlaceOrder(
        [FromBody] PlaceOrderDto request,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        VoidResult placeOrderResult = await orderService.PlaceOrderAsync(request, ct);

        if (placeOrderResult.IsFailure)
        {
            return placeOrderResult.StatusCode switch
            {
                HttpStatusCode.BadRequest => TypedResults.BadRequest(placeOrderResult.ErrorMessage),
                HttpStatusCode.NotFound => TypedResults.NotFound(placeOrderResult.ErrorMessage),
                _ => throw new Exception($"Endpoint: {nameof(PlaceOrder)} -> Unknown status code: {placeOrderResult.StatusCode}")
            };
        }

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, BadRequest<string>>> MarkOrderPaid(
        [FromRoute] Guid orderId,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        VoidResult markOrderPaidResult = await orderService.MarkOrderPaidAsync(orderId, ct);
        
        if (markOrderPaidResult.IsFailure)
        {
            return TypedResults.BadRequest(markOrderPaidResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }
}