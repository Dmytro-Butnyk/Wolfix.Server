namespace Support.Application.Dto.Support;

public sealed record CreateSupportDto(string Email, string Password, string FirstName, string LastName, string MiddleName);