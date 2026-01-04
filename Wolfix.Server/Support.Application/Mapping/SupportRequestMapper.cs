using Support.Application.Dto;
using Support.Domain.Projections;

namespace Support.Application.Mapping;

internal static class SupportRequestMapper
{
    public static SupportRequestShortDto ToShortDto(this SupportRequestShortProjection projection)
        => new(projection.Id, projection.Category, projection.RequestContent, projection.CreatedAt);
}