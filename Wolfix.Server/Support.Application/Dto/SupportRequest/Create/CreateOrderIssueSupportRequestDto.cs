namespace Support.Application.Dto.SupportRequest.Create;

public sealed record CreateOrderIssueSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate,
    Guid CustomerId,
    string Category,
    string Content,
    Guid OrderId
) : CreateSupportRequestDto(FirstName, LastName, MiddleName, PhoneNumber, BirthDate, CustomerId, Category, Content);