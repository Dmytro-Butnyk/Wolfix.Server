using Shared.Application.Dto;

namespace Customer.Application.Dto.CartItem;

public sealed record CartItemDto(Guid Id, string PhotoUrl, string Title, decimal Price);