using Order.Application.Contracts;
using Order.Application.Dto.Order.Requests;
using Order.Application.Interfaces;
using Order.Application.Models;
using Order.Domain.Interfaces.Order;
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
    public async Task<Result<string>> PlaceOrderAsync(PlaceOrderDto request, CancellationToken ct)
    {
        VoidResult checkCustomerExistResult = await eventBus.PublishAsync(new CustomerWantsToPlaceOrder
        {
            CustomerId = request.Order.CustomerId
        }, ct);

        if (checkCustomerExistResult.IsFailure)
        {
            return Result<string>.Failure(checkCustomerExistResult);
        }
        
        var orderData = request.Order;
        
        Result<OrderAggregate> createOrderResult = OrderAggregate.Create(orderData.CustomerFirstName, orderData.CustomerLastName,
            orderData.CustomerMiddleName, orderData.CustomerPhoneNumber, orderData.CustomerEmail, orderData.CustomerId,
            orderData.RecipientFirstName, orderData.RecipientLastName, orderData.RecipientMiddleName, orderData.RecipientPhoneNumber,
            orderData.PaymentOption, orderData.PaymentStatus, orderData.DeliveryMethodName, orderData.DeliveryInfoNumber,
            orderData.DeliveryInfoCity, orderData.DeliveryInfoStreet, orderData.DeliveryInfoHouseNumber, orderData.DeliveryOption,
            orderData.WithBonuses, orderData.UsedBonusesAmount, orderData.Price);

        if (createOrderResult.IsFailure)
        {
            return Result<string>.Failure(createOrderResult);
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
            return Result<string>.Failure(checkProductsExistResult);
        }
        
        foreach (var orderItem in request.OrderItems)
        {
            VoidResult addOrderItemResult = order.AddOrderItem(orderItem.ProductId, orderItem.PhotoUrl, orderItem.Title,
                orderItem.Quantity, orderItem.Price);

            if (addOrderItemResult.IsFailure)
            {
                return Result<string>.Failure(addOrderItemResult);
            }
        }
        
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

        order.AddPaymentIntentId(paymentResponse.PaymentIntentId);
        
        await orderRepository.AddAsync(order, ct);
        await orderRepository.SaveChangesAsync(ct);
        
        return Result<string>.Success(paymentResponse.ClientSecret);
    }
}