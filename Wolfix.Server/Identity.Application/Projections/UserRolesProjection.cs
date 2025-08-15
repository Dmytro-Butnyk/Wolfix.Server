namespace Identity.Application.Projections;

public sealed record UserRolesProjection(Guid UserId, string Email, IList<string> Roles);