using Support.Domain.Enums;

namespace Support.Application.Dto.SupportRequest.Create;

public sealed record CreateOrderIssueSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate,
    Guid CustomerId,
    string Content,
    Guid OrderId
) : CreateSupportRequestDto(FirstName, LastName, MiddleName, PhoneNumber, BirthDate, CustomerId, nameof(SupportRequestCategory.OrderIssue), Content);