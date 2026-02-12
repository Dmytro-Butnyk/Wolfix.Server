namespace Catalog.Application.Dto.Discount.Requests;

public sealed record AddDiscountDto(uint Percent, DateTime ExpirationDateTime);