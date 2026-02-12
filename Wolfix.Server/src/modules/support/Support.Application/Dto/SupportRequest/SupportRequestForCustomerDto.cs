using Support.Domain.Projections;

namespace Support.Application.Dto.SupportRequest;

public sealed record SupportRequestForCustomerDto(Guid Id, string Category, string RequestContent, string Status,
    string? ResponseContent, DateTime CreatedAt, DateTime? ProcessedAt, List<SupportRequestAdditionalProperty> AdditionalProperties);