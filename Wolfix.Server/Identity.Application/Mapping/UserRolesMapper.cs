using Identity.Application.Dto.Responses;
using Identity.Application.Projections;

namespace Identity.Application.Mapping;

internal static class UserRolesMapper
{
    public static UserRolesDto ToDto(this UserRolesProjection projection)
        => new(projection.UserId, projection.Email, projection.Roles);
}