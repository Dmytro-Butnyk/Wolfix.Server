using System.Net;
using Order.Application.Contracts;
using Order.Application.Dto.Order.Requests;
using Order.Application.Dto.Order.Responses;
using Order.Application.Dto.OrderItem.Responses;
using Order.Application.Mapping;
using Order.Application.Models;
using Order.Domain.Interfaces.Order;
using Order.Domain.OrderAggregate.Enums;
using Order.Domain.Projections;
using Order.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using OrderAggregate = Order.Domain.OrderAggregate.Order;

namespace Order.Application.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IPaymentService<StripePaymentResponse> paymentService,
    EventBus eventBus)
{
    public async Task<Result<OrderPlacedWithPaymentDto>> PlaceOrderWithPaymentAsync(PlaceOrderDto request, CancellationToken ct)
    {
        Result<OrderAggregate> createOrderResult = await CreateOrderAsync(request, true, ct);
        
        if (createOrderResult.IsFailure)
        {
            return Result<OrderPlacedWithPaymentDto>.Failure(createOrderResult);
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
            return Result<OrderPlacedWithPaymentDto>.Failure(payResult);
        }
        
        StripePaymentResponse paymentResponse = payResult.Value!;

        VoidResult addPaymentIntentIdResult = order.AddPaymentIntentId(paymentResponse.PaymentIntentId);

        if (addPaymentIntentIdResult.IsFailure)
        {
            return Result<OrderPlacedWithPaymentDto>.Failure(addPaymentIntentIdResult);
        }
        
        await orderRepository.AddAsync(order, ct);
        await orderRepository.SaveChangesAsync(ct);
        
        VoidResult deleteCustomerCartItemsResult = await DeleteCartItemsAsync(order, ct);

        if (deleteCustomerCartItemsResult.IsFailure)
        {
            return Result<OrderPlacedWithPaymentDto>.Failure(deleteCustomerCartItemsResult);
        }
        
        OrderPlacedWithPaymentDto dto = new(paymentResponse.ClientSecret, order.Id);
        
        return Result<OrderPlacedWithPaymentDto>.Success(dto);
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

        VoidResult deleteCustomerCartItemsResult = await DeleteCartItemsAsync(createOrderResult.Value!, ct);

        if (deleteCustomerCartItemsResult.IsFailure)
        {
            return VoidResult.Failure(deleteCustomerCartItemsResult);
        }
        
        return VoidResult.Success();
    }
    
    private async Task<Result<OrderAggregate>> CreateOrderAsync(PlaceOrderDto request, bool payNow, CancellationToken ct)
    {
        VoidResult checkCustomerExistResult = await eventBus.PublishWithoutResultAsync(new CustomerWantsToPlaceOrder
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
        
        VoidResult checkProductsExistResult = await eventBus.PublishWithoutResultAsync(new CustomerWantsToPlaceOrderItems
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
            VoidResult addOrderItemResult = order.AddOrderItem(orderItem.ProductId, orderItem.SellerId, orderItem.CartItemId, orderItem.PhotoUrl, orderItem.Title,
                orderItem.Quantity, orderItem.Price);

            if (addOrderItemResult.IsFailure)
            {
                return Result<OrderAggregate>.Failure(addOrderItemResult);
            }
        }
        
        return Result<OrderAggregate>.Success(order);
    }

    private async Task<VoidResult> DeleteCartItemsAsync(OrderAggregate order, CancellationToken ct)
    {
        List<Guid> cartItemsIds = order.OrderItems
            .Select(oi => oi.CartItemId)
            .ToList();

        VoidResult deleteCustomerCartItemsResult = await eventBus.PublishWithoutResultAsync(new CustomerOrderCreated
        {
            CartItemsIds = cartItemsIds,
            CustomerId = order.CustomerId
        }, ct);

        if (deleteCustomerCartItemsResult.IsFailure)
        {
            return VoidResult.Failure(deleteCustomerCartItemsResult);
        }
        
        return VoidResult.Success();
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

    public async Task<Result<IReadOnlyCollection<CustomerOrderDto>>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct)
    {
        VoidResult checkCustomerExistResult = await eventBus.PublishWithoutResultAsync(new CustomerWantsToGetOrders
        {
            CustomerId = customerId
        }, ct);

        if (checkCustomerExistResult.IsFailure)
        {
            return Result<IReadOnlyCollection<CustomerOrderDto>>.Failure(checkCustomerExistResult);
        }
        
        IReadOnlyCollection<CustomerOrderProjection> customerOrders = await orderRepository.GetCustomerOrdersAsync(customerId, ct);

        IReadOnlyCollection<CustomerOrderDto> dto = customerOrders
            .Select(order => order.ToCustomerShortDto())
            .ToList();

        return Result<IReadOnlyCollection<CustomerOrderDto>>.Success(dto);
    }

    public async Task<Result<OrderDetailsDto>> GetOrderDetailsAsync(Guid orderId, CancellationToken ct)
    {
        OrderDetailsProjection? projection = await orderRepository.GetOrderDetailsAsync(orderId, ct);

        if (projection is null)
        {
            return Result<OrderDetailsDto>.Failure(
                $"Order with id: {orderId} not found",
                HttpStatusCode.NotFound
            );
        }

        OrderDetailsDto dto = projection.ToCustomerDetailsDto();
        
        return Result<OrderDetailsDto>.Success(dto);
    }

    public async Task<Result<IReadOnlyCollection<SellerOrderItemDto>>> GetSellerOrdersAsync(Guid sellerId, CancellationToken ct)
    {
        VoidResult checkSellerExistResult = await eventBus.PublishWithoutResultAsync(new CheckSellerExist(sellerId), ct);

        if (checkSellerExistResult.IsFailure)
        {
            return Result<IReadOnlyCollection<SellerOrderItemDto>>.Failure(checkSellerExistResult);
        }
        
        IReadOnlyCollection<SellerOrderItemProjection> sellerOrders = await orderRepository.GetSellerOrdersAsync(sellerId, ct);

        IReadOnlyCollection<SellerOrderItemDto> dto = sellerOrders
            .Select(order => order.ToSellerShortDto())
            .ToList();

        return Result<IReadOnlyCollection<SellerOrderItemDto>>.Success(dto);
    }
}