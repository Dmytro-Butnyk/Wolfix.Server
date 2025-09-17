using Order.Domain.OrderAggregate.Enums;

namespace Order.Application.Dto.Order.Requests;

public sealed record CreateOrderDto(string CustomerFirstName, string CustomerLastName, string CustomerMiddleName,
    string CustomerPhoneNumber, string CustomerEmail, Guid CustomerId, string RecipientFirstName, string RecipientLastName,
    string RecipientMiddleName, string RecipientPhoneNumber, OrderPaymentOption PaymentOption, OrderPaymentStatus PaymentStatus,
    string DeliveryMethodName, uint? DeliveryInfoNumber, string DeliveryInfoCity, string DeliveryInfoStreet,
    uint DeliveryInfoHouseNumber, DeliveryOption DeliveryOption,
    bool WithBonuses, decimal UsedBonusesAmount, decimal Price);