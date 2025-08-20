namespace Customer.Application.Dto;

public sealed record AddProductToFavoriteDto(Guid CustomerId, Guid ProductId);