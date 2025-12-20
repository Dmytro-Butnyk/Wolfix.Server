using Shared.Domain.ValueObjects;

namespace Support.Domain.Projections;

public sealed record SupportRequestShortProjection(Guid Id, Email Email, FullName FullName, PhoneNumber PhoneNumber,
    string Title, DateTime CreatedAt);