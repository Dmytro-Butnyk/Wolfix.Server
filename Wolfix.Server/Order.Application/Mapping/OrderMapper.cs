using Order.Application.Dto.Order.Responses;
using Order.Domain.Projections;

namespace Order.Application.Mapping;

internal static class OrderMapper
{
    public static CustomerOrderDto ToCustomerShortDto(this CustomerOrderProjection projection)
        => new(projection.Id, projection.Number, projection.ProductsNames, projection.DeliveryStatus, projection.PaymentOption, projection.PaymentStatus,
            projection.DeliveryInfo, projection.DeliveryMethodName, projection.Price,
            projection.CreatedAt);

    public static OrderDetailsDto ToCustomerDetailsDto(this OrderDetailsProjection projection)
        => new(projection.Id, projection.Number, projection.RecipientFirstName, projection.RecipientLastName,
            projection.RecipientMiddleName,
            projection.RecipientPhoneNumber, projection.DeliveryStatus, projection.PaymentOption,
            projection.PaymentStatus, projection.DeliveryNumber,
            projection.DeliveryCity, projection.DeliveryStreet, projection.DeliveryHouseNumber,
            projection.DeliveryMethodName, projection.Price,
            projection.OrderItems.Select(oi =>
                new OrderItemDetailsDto(oi.Id, oi.PhotoUrl, oi.Title, oi.Quantity, oi.Price, oi.ProductId)).ToList());
}