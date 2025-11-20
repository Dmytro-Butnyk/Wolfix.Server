using Order.Domain.OrderAggregate.Enums;

namespace Order.Application.Dto.Order.Requests;

public sealed record PlaceOrderDto(CreateOrderDto Order, IReadOnlyCollection<CreateOrderItemDto> OrderItems);