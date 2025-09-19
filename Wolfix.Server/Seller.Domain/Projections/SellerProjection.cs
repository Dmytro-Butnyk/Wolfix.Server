using Shared.Domain.ValueObjects;

namespace Seller.Domain.Projections;

public sealed record SellerProjection(Guid Id, string? PhotoUrl, FullName FullName, string PhoneNumber, Address Address, DateOnly BirthDate);