namespace Order.Application.Dto.Order.Responses;

public sealed record OrderPlacedWithPaymentDto(string ClientSecret, Guid OrderId);