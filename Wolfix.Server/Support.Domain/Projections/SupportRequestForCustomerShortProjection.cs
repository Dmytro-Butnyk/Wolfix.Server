using Support.Domain.Enums;

namespace Support.Domain.Projections;

public sealed record SupportRequestForCustomerShortProjection(Guid Id, SupportRequestCategory Category, string RequestContent, SupportRequestStatus Status, DateTime CreatedAt);