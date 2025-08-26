using Customer.Application.Dto.FavoriteItem;
using Customer.Domain.Projections;

namespace Customer.Application.Mapping.FavoriteItem;

internal static class FavoriteItemMapper
{
    public static FavoriteItemDto ToDto(this FavoriteItemProjection projection)
        => new(projection.Id, projection.PhotoUrl, projection.Title, projection.AverageRating,
            projection.Price, projection.FinalPrice, projection.Bonuses, projection.CustomerId);
}