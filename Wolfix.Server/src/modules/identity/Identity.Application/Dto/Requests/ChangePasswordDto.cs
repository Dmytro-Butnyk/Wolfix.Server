namespace Identity.Application.Dto.Requests;

public sealed record ChangePasswordDto(string CurrentPassword, string NewPassword);