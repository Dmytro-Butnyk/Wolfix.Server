using Shared.Application.Dto;
using Shared.Domain.ValueObjects;

namespace Seller.Application.Dto;

public sealed record SellerDto(Guid Id, string? PhotoUrl, FullName FullName, string PhoneNumber, Address Address, DateOnly BirthDate);