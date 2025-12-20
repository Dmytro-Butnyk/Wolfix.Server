namespace Support.Application.Dto;

public sealed record CreateSupportRequestDto(string Email, string FirstName, string LastName, string MiddleName, string PhoneNumber,
    DateOnly BirthDate, Guid CustomerId, string Title, string Content, Guid? ProductId = null);