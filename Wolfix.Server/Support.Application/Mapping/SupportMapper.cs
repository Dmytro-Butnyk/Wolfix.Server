using Support.Application.Dto;
using Support.Domain.Projections;

namespace Support.Application.Mapping;

internal static class SupportMapper
{
    public static SupportForAdminDto ToAdminDto(this SupportForAdminProjection projection)
        => new(projection.Id, projection.FullName.FirstName, projection.FullName.LastName,
            projection.FullName.MiddleName);
}