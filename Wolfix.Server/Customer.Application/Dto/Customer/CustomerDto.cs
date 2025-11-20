using Shared.Domain.ValueObjects;

namespace Customer.Application.Dto.Customer;

public sealed record CustomerDto(Guid Id, string? PhotoUrl, FullName? FullName, string? PhoneNumber,
    Address? Address, DateOnly? BirthDate, decimal BonusesAmount);