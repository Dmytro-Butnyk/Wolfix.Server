namespace Order.Application.Dto.Order.Requests;

public sealed record CreateOrderItemDto(Guid ProductId, string PhotoUrl, string Title, uint Quantity, decimal Price);