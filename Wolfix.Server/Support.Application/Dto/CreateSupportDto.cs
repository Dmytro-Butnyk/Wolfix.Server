namespace Support.Application.Dto;

public sealed record CreateSupportDto(string Email, string Password, string FirstName, string LastName, string MiddleName);