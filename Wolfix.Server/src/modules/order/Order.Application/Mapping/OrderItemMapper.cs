using Order.Application.Dto.OrderItem.Responses;
using Order.Domain.Projections;

namespace Order.Application.Mapping;

internal static class OrderItemMapper
{
    public static SellerOrderItemDto ToSellerShortDto(this SellerOrderItemProjection projection)
        => new(projection.Id, projection.Title, projection.Price, projection.Quantity, projection.PhotoUrl);
}