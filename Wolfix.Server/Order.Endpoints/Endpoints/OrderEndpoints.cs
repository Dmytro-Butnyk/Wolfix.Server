using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Order.Application.Dto.Order.Requests;
using Order.Application.Dto.Order.Responses;
using Order.Application.Dto.OrderItem.Responses;
using Order.Application.Interfaces;
using Order.Domain.DeliveryAggregate;
using Shared.Domain.Models;

namespace Order.Endpoints.Endpoints;

internal static class OrderEndpoints
{
    private const string Route = "api/orders";

    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orderGroup = app.MapGroup(Route)
            .WithTags("Order");
        
        orderGroup.MapGet("{orderId:guid}/details", GetOrderDetails)
            .RequireAuthorization("Customer")
            .WithSummary("Get order details");

        //todo: переделать роут на: /api/orders/customers/{customerId:guid}
        //todo: та и вообще перенести заказы пользователя в модуль пользователей а заказы продавца в модуль продавца
        orderGroup.MapGet("{customerId:guid}", GetCustomerOrders)
            .RequireAuthorization("Customer")
            .WithSummary("Get all orders by specific customer");
        
        orderGroup.MapGet("sellers/{sellerId:guid}", GetSellerOrders)
            .RequireAuthorization("Seller")
            .WithSummary("Get all orders by specific seller");
        
        orderGroup.MapPost("with-payment", PlaceOrderWithPayment)
            .RequireAuthorization("Customer")
            .WithSummary("Creates an order and returns client secret for payment");

        orderGroup.MapPatch("{orderId:guid}/paid", MarkOrderPaid)
            .RequireAuthorization("Customer")
            .WithSummary("Marks order as paid");

        orderGroup.MapPost("", PlaceOrder)
            .RequireAuthorization("Customer")
            .WithSummary("Creates an order without payment");
    }

    private static async Task<Results<Ok<OrderDetailsDto>, NotFound<string>>> GetOrderDetails(
        [FromRoute] Guid orderId,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        Result<OrderDetailsDto> getOrderDetailsResult = await orderService.GetOrderDetailsAsync(orderId, ct);

        if (getOrderDetailsResult.IsFailure)
        {
            return TypedResults.NotFound(getOrderDetailsResult.ErrorMessage);
        }

        return TypedResults.Ok(getOrderDetailsResult.Value!);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<CustomerOrderDto>>, NotFound<string>>> GetCustomerOrders(
        [FromRoute] Guid customerId,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        Result<IReadOnlyCollection<CustomerOrderDto>> getCustomerOrders = await orderService.GetCustomerOrdersAsync(customerId, ct);

        if (getCustomerOrders.IsFailure)
        {
            return TypedResults.NotFound(getCustomerOrders.ErrorMessage);
        }

        return TypedResults.Ok(getCustomerOrders.Value!);
    }

    private static async Task<Results<Ok<IReadOnlyCollection<SellerOrderItemDto>>, NotFound<string>>> GetSellerOrders(
        [FromRoute] Guid sellerId,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        Result<IReadOnlyCollection<SellerOrderItemDto>> getSellerOrders = await orderService.GetSellerOrdersAsync(sellerId, ct);

        if (getSellerOrders.IsFailure)
        {
            return TypedResults.NotFound(getSellerOrders.ErrorMessage);
        }

        return TypedResults.Ok(getSellerOrders.Value!);
    }

    //todo: добавить проверку чтобы не создавать повторно такой же заказ
    //todo: если перешёл на страницу оплаты, не оплатил и вышел то помечается как оплачено
    private static async Task<Results<Ok<OrderPlacedWithPaymentDto>, BadRequest<string>, NotFound<string>, InternalServerError<string>>> PlaceOrderWithPayment(
        [FromBody] PlaceOrderDto request,
        [FromServices] IOrderService orderService,
        CancellationToken ct)
    {
        Result<OrderPlacedWithPaymentDto> placeOrderWithPaymentResult = await orderService.PlaceOrderWithPaymentAsync(request, ct);

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

    //todo: добавить проверку чтобы не создавать повторно такой же заказ
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