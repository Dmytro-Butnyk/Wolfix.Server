using Shared.Domain.ValueObjects;

namespace Support.Domain.Projections;

public sealed record SupportRequestShortProjection(Guid Id, string Category, string RequestContent,
    DateTime CreatedAt, List<SupportRequestAdditionalProperty> AdditionalProperties);