namespace Customer.Application.Dto;

public sealed record AddProductToFavoriteDto(Guid CustomerId, string Title, string PhotoUrl, decimal Price,
    uint Bonuses, double? AverageRating, decimal? FinalPrice);