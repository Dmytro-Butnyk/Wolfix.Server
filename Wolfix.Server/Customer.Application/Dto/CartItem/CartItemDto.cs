namespace Customer.Application.Dto.CartItem;

public sealed record CartItemDto(Guid Id, Guid ProductId, string PhotoUrl, string Title, decimal Price);