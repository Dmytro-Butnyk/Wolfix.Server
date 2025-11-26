namespace Order.Application.Dto.Order.Responses;

public sealed record OrderDetailsDto(Guid Id, string Number, string RecipientFirstName, string RecipientLastName,
    string RecipientMiddleName, string RecipientPhoneNumber, string DeliveryStatus, string PaymentOption, string PaymentStatus,
    uint? DeliveryNumber, string DeliveryCity, string DeliveryStreet, uint DeliveryHouseNumber, string DeliveryMethodName,
    decimal Price, IReadOnlyCollection<OrderItemDetailsDto> OrderItems);