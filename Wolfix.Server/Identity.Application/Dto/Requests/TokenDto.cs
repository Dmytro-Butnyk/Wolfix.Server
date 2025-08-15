namespace Identity.Application.Dto.Requests;

public sealed record TokenDto(Guid UserId, string Email, string Role);