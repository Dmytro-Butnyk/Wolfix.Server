using Admin.Application.Dto.Responses;
using Admin.Domain.Projections;

namespace Admin.Application.Mapping;

internal static class AdminMapper
{
    public static BasicAdminDto ToDto(this BasicAdminProjection projection)
        => new(projection.Id, projection.FullName.FirstName, projection.FullName.LastName,
            projection.FullName.MiddleName, projection.PhoneNumber.Value);
}