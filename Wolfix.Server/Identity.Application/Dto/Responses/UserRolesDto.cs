namespace Identity.Application.Dto.Responses;

public sealed record UserRolesDto(Guid UserId, string Email, IList<string> Roles);