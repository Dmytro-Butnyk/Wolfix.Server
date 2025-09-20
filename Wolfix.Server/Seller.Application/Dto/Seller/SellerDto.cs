using Shared.Domain.ValueObjects;

namespace Seller.Application.Dto.Seller;

public sealed record SellerDto(Guid Id, string? PhotoUrl, FullName FullName, string PhoneNumber, Address Address, DateOnly BirthDate);