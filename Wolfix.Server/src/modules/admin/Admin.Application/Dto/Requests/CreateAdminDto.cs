namespace Admin.Application.Dto.Requests;

public sealed record CreateAdminDto(string Email, string Password, string FirstName, string LastName,
    string MiddleName, string PhoneNumber);