using Order.Domain.OrderAggregate.Enums;
using Order.Domain.OrderAggregate.ValueObjects;

namespace Order.Domain.Projections;

public sealed record SellerOrderItemProjection(Guid Id, string Title, decimal Price, uint Quantity, string PhotoUrl);