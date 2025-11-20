namespace Customer.Application.Dto.FavoriteItem;

public sealed record FavoriteItemDto(Guid Id, string PhotoUrl, string Title, double? AverageRating, decimal Price,
    decimal? FinalPrice, uint Bonuses, Guid CustomerId);