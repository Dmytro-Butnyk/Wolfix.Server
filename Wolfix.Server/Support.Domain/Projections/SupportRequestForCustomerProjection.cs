using Support.Domain.Enums;

namespace Support.Domain.Projections;

public sealed record SupportRequestForCustomerProjection(Guid Id, SupportRequestCategory Category,
    string RequestContent, SupportRequestStatus Status, string? ResponseContent, DateTime CreatedAt,
    DateTime? ProcessedAt, List<SupportRequestAdditionalProperty> AdditionalProperties);