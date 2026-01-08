using Support.Application.Dto;
using Support.Application.Dto.SupportRequest;
using Support.Domain.Projections;

namespace Support.Application.Mapping;

internal static class SupportRequestMapper
{
    public static SupportRequestShortDto ToShortDto(this SupportRequestShortProjection projection)
        => new(projection.Id, projection.Category, projection.RequestContent, projection.CreatedAt);
    
    public static SupportRequestForCustomerShortDto ToCustomerShortDto(this SupportRequestForCustomerShortProjection projection)
        => new(projection.Id, projection.Category.ToString(), projection.RequestContent, projection.Status.ToString(), projection.CreatedAt);

    public static SupportRequestForCustomerDto ToCustomerDto(this SupportRequestForCustomerProjection projection)
        => new(projection.Id, projection.Category.ToString(), projection.RequestContent, projection.Status.ToString(),
            projection.ResponseContent, projection.CreatedAt, projection.ProcessedAt, projection.AdditionalProperties);
}