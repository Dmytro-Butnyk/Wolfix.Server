namespace Customer.Application.Dto.Product;

public sealed record AddProductToFavoriteDto(Guid CustomerId, Guid ProductId);