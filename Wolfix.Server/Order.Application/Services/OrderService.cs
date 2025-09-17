using Order.Application.Contracts;
using Order.Application.Dto.Order.Requests;
using Order.Application.Interfaces;
using Order.Application.Models;
using Order.Domain.Interfaces.Order;
using Shared.Domain.Models;
using OrderAggregate = Order.Domain.OrderAggregate.Order;

namespace Order.Application.Services;

internal sealed class OrderService(IOrderRepository orderRepository, IPaymentService paymentService) : IOrderService
{
    public async Task<Result<string>> PlaceOrderAsync(PlaceOrderDto request, CancellationToken ct)
    {
        //todo: проверять есть ли такой пользователь по айди через событие
        
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
        
        foreach (var orderItem in request.OrderItems)
        {
            //todo: проверять есть ли такой продукт по продакт айди через событие
            
            VoidResult addOrderItemResult = order.AddOrderItem(orderItem.ProductId, orderItem.PhotoUrl, orderItem.Title,
                orderItem.Quantity, orderItem.Price);

            if (addOrderItemResult.IsFailure)
            {
                return Result<string>.Failure(addOrderItemResult);
            }
        }
        
        Result<StripePaymentResponse> payResult = await paymentService.PayAsync(order.Price, "uah", order.CustomerInfo.Email.Value, ct);

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