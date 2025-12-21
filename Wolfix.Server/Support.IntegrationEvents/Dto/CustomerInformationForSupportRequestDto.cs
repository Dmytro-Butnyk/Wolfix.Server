namespace Support.IntegrationEvents.Dto;

public sealed record CustomerInformationForSupportRequestDto(
    string FirstName,
    string LastName,
    string MiddleName,
    string PhoneNumber,
    DateOnly? BirthDate);