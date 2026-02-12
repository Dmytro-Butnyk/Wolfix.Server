using Support.Domain.Projections;

namespace Support.Application.Dto.SupportRequest;

public sealed record SupportRequestShortDto(Guid Id, string Category, string RequestContent,
    DateTime CreatedAt, List<SupportRequestAdditionalProperty> AdditionalProperties);