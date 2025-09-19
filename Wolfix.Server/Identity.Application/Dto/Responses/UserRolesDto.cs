namespace Identity.Application.Dto.Responses;

public sealed record UserRolesDto(Guid AccountId, string Email, IList<string> Roles);