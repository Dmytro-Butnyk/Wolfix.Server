using Support.Domain.Enums;

namespace Support.Application.Dto.SupportRequest.Create;

public sealed record CreateGeneralSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate,
    Guid CustomerId,
    string Content
) : CreateSupportRequestDto(FirstName, LastName, MiddleName, PhoneNumber, BirthDate, CustomerId, nameof(SupportRequestCategory.General), Content);