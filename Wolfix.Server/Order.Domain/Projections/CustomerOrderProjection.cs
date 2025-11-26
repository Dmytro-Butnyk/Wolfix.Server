using Order.Domain.OrderAggregate.Enums;
using Order.Domain.OrderAggregate.ValueObjects;

namespace Order.Domain.Projections;

public sealed record CustomerOrderProjection(Guid Id, string Number, List<string> ProductsNames, OrderDeliveryStatus DeliveryStatus, OrderPaymentOption PaymentOption, OrderPaymentStatus PaymentStatus,
    DeliveryInfo DeliveryInfo, string DeliveryMethodName, decimal Price, DateTime CreatedAt);