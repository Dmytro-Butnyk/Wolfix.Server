namespace Order.Domain.Projections;

public sealed record OrderDetailsProjection(Guid Id, string Number, string RecipientFirstName, string RecipientLastName,
    string RecipientMiddleName, string RecipientPhoneNumber, string DeliveryStatus, string PaymentOption, string PaymentStatus,
    uint? DeliveryNumber, string DeliveryCity, string DeliveryStreet, uint DeliveryHouseNumber, string DeliveryMethodName,
    decimal Price, IReadOnlyCollection<OrderItemDetailsProjection> OrderItems);
    