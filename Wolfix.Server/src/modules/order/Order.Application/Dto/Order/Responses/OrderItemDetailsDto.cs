namespace Order.Application.Dto.Order.Responses;

public sealed record OrderItemDetailsDto(Guid Id, string PhotoUrl, string Title, uint Quantity, decimal Price, Guid ProductId);