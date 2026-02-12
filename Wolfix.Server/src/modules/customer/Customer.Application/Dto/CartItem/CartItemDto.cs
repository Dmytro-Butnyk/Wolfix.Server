namespace Customer.Application.Dto.CartItem;

public sealed record CartItemDto(Guid Id, Guid ProductId, Guid SellerId, string PhotoUrl, string Title, decimal Price);