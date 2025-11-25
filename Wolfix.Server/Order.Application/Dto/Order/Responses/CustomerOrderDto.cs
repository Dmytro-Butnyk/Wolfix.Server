using Order.Domain.OrderAggregate.Enums;
using Order.Domain.OrderAggregate.ValueObjects;

namespace Order.Application.Dto.Order.Responses;

public sealed record CustomerOrderDto(Guid Id, string Number, OrderPaymentOption PaymentOption, OrderPaymentStatus PaymentStatus,
    DeliveryInfo DeliveryInfo, string DeliveryMethodName, decimal Price, DateTime CreatedAt);