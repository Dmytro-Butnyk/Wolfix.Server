using Customer.Application.Dto.CartItem;
using Customer.Domain.Projections;

namespace Customer.Application.Mapping.CartItem;

internal static class CartItemMapper
{
    public static CartItemDto ToDto(this CartItemProjection projection)
        => new(projection.Id, projection.CustomerId, projection.PhotoUrl, projection.Title, projection.Price);
}