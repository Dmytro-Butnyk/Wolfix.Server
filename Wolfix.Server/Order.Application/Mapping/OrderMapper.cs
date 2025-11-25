using Order.Application.Dto.Order.Responses;
using Order.Domain.Projections;

namespace Order.Application.Mapping;

internal static class OrderMapper
{
    public static CustomerOrderDto ToDto(this CustomerOrderProjection projection)
        => new(projection.Id, projection.Number, projection.DeliveryStatus, projection.PaymentOption, projection.PaymentStatus,
            projection.DeliveryInfo, projection.DeliveryMethodName, projection.Price,
            projection.CreatedAt);
}