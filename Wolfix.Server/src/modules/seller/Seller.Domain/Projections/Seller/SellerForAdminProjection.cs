using Shared.Domain.ValueObjects;

namespace Seller.Domain.Projections.Seller;

public sealed record SellerForAdminProjection(Guid Id, string? PhotoUrl, FullName FullName, string PhoneNumber, Address Address,
    DateOnly BirthDate, IReadOnlyCollection<string> Categories);