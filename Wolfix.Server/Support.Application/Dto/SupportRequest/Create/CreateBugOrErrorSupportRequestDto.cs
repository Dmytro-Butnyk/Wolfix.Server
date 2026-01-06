namespace Support.Application.Dto.SupportRequest.Create;

public sealed record CreateBugOrErrorSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate,
    Guid CustomerId,
    string Category,
    string Content,
    string PhotoUrl
) : CreateSupportRequestDto(FirstName, LastName, MiddleName, PhoneNumber, BirthDate, CustomerId, Category, Content);