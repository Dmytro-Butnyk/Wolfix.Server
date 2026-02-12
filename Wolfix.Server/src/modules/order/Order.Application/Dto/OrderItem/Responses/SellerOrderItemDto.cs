namespace Order.Application.Dto.OrderItem.Responses;

public sealed record SellerOrderItemDto(Guid Id, string Title, decimal Price, uint Quantity, string PhotoUrl);