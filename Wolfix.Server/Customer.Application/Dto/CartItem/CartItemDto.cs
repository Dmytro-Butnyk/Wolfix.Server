using Shared.Application.Dto;

namespace Customer.Application.Dto.CartItem;

public sealed record CartItemDto(Guid Id, Guid CustomerId, string PhotoUrl, string Title, decimal Price);