using Shared.Domain.Projections;

namespace Customer.Domain.Projections;

public sealed record FavoriteItemProjection(Guid Id, string PhotoUrl, string Title, double? AverageRating, decimal Price,
    decimal? FinalPrice, uint Bonuses, Guid CustomerId) : BaseProjection(Id);