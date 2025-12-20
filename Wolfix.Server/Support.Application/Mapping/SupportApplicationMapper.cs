using Support.Application.Dto;
using Support.Domain.Projections;

namespace Support.Application.Mapping;

internal static class SupportApplicationMapper
{
    public static SupportRequestShortDto ToShortDto(this SupportRequestShortProjection projection)
        => new(projection.Id, projection.Email, projection.FullName, projection.PhoneNumber,
            projection.Title, projection.CreatedAt);
}