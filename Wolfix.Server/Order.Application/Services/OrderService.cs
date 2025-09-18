using System.Net;
using Order.Application.Contracts;
using Order.Application.Dto.Order.Requests;
using Order.Application.Interfaces;
using Order.Application.Models;
using Order.Domain.Interfaces.Order;
using Order.Domain.OrderAggregate.Enums;
using Order.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using OrderAggregate = Order.Domain.OrderAggregate.Order;

namespace Order.Application.Services;

internal sealed class OrderService(
    IOrderRepository orderRepository,
    IPaymentService<StripePaymentResponse> paymentService,
    IEventBus eventBus) : IOrderService
{
    public async Task<Result<string>> PlaceOrderWithPaymentAsync(PlaceOrderDto request, CancellationToken ct)
    {
        Result<OrderAggregate> createOrderResult = await CreateOrderAsync(request, true, ct);
        
        if (createOrderResult.IsFailure)
        {
            return Result<string>.Failure(createOrderResult);
        }
        
        OrderAggregate order = createOrderResult.Value!;
        
        Result<StripePaymentResponse> payResult = await paymentService.PayAsync(
            order.Price,
            "uah",
            order.CustomerInfo.Email.Value,
            ct
        );

        if (payResult.IsFailure)
        {
            return Result<string>.Failure(payResult);
        }
        
        StripePaymentResponse paymentResponse = payResult.Value!;

        VoidResult addPaymentIntentIdResult = order.AddPaymentIntentId(paymentResponse.PaymentIntentId);

        if (addPaymentIntentIdResult.IsFailure)
        {
            return Result<string>.Failure(addPaymentIntentIdResult);
        }
        
        await orderRepository.AddAsync(order, ct);
        await orderRepository.SaveChangesAsync(ct);
        
        return Result<string>.Success(paymentResponse.ClientSecret);
    }

    public async Task<VoidResult> PlaceOrderAsync(PlaceOrderDto request, CancellationToken ct)
    {
        Result<OrderAggregate> createOrderResult = await CreateOrderAsync(request, false, ct);
        
        if (createOrderResult.IsFailure)
        {
            return VoidResult.Failure(createOrderResult);
        }
        
        await orderRepository.AddAsync(createOrderResult.Value!, ct);
        await orderRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
    
    private async Task<Result<OrderAggregate>> CreateOrderAsync(PlaceOrderDto request, bool payNow, CancellationToken ct)
    {
        VoidResult checkCustomerExistResult = await eventBus.PublishAsync(new CustomerWantsToPlaceOrder
        {
            CustomerId = request.Order.CustomerId
        }, ct);

        if (checkCustomerExistResult.IsFailure)
        {
            return Result<OrderAggregate>.Failure(checkCustomerExistResult);
        }
        
        var orderData = request.Order;

        (OrderPaymentOption paymentOption, OrderPaymentStatus paymentStatus) = payNow switch
        {
            true => (OrderPaymentOption.Card, OrderPaymentStatus.Pending),
            false => (OrderPaymentOption.WhileReceiving, OrderPaymentStatus.Unpaid)
        };
        
        Result<OrderAggregate> createOrderResult = OrderAggregate.Create(orderData.CustomerFirstName, orderData.CustomerLastName,
            orderData.CustomerMiddleName, orderData.CustomerPhoneNumber, orderData.CustomerEmail, orderData.CustomerId,
            orderData.RecipientFirstName, orderData.RecipientLastName, orderData.RecipientMiddleName, orderData.RecipientPhoneNumber,
            paymentOption, paymentStatus, orderData.DeliveryMethodName, orderData.DeliveryInfoNumber,
            orderData.DeliveryInfoCity, orderData.DeliveryInfoStreet, orderData.DeliveryInfoHouseNumber, orderData.DeliveryOption,
            orderData.WithBonuses, orderData.UsedBonusesAmount, orderData.Price);

        if (createOrderResult.IsFailure)
        {
            return Result<OrderAggregate>.Failure(createOrderResult);
        }
        
        OrderAggregate order = createOrderResult.Value!;
        
        VoidResult checkProductsExistResult = await eventBus.PublishAsync(new CustomerWantsToPlaceOrderItems
        {
            ProductIds = request.OrderItems
                .Select(orderItem => orderItem.ProductId)
                .ToList()
        }, ct);

        if (checkProductsExistResult.IsFailure)
        {
            return Result<OrderAggregate>.Failure(checkProductsExistResult);
        }
        
        foreach (var orderItem in request.OrderItems)
        {
            VoidResult addOrderItemResult = order.AddOrderItem(orderItem.ProductId, orderItem.PhotoUrl, orderItem.Title,
                orderItem.Quantity, orderItem.Price);

            if (addOrderItemResult.IsFailure)
            {
                return Result<OrderAggregate>.Failure(addOrderItemResult);
            }
        }
        
        return Result<OrderAggregate>.Success(order);
    }

    public async Task<VoidResult> MarkOrderPaidAsync(Guid orderId, CancellationToken ct)
    {
        OrderAggregate? order = await orderRepository.GetByIdAsync(orderId, ct);

        if (order is null)
        {
            return VoidResult.Failure(
                $"Order with id: {orderId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult markOrderPaidResult = order.MarkAsPaid();

        if (markOrderPaidResult.IsFailure)
        {
            return VoidResult.Failure(markOrderPaidResult);
        }
        
        await orderRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}