namespace Customer.Application.Dto.Product;

public sealed record AddProductToCartDto(Guid CustomerId, Guid ProductId);