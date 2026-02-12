using Shared.Domain.ValueObjects;

namespace Customer.Domain.Projections;

public sealed record CustomerProjection(Guid Id, string? PhotoUrl, FullName? FullName, string? PhoneNumber,
    Address? Address, DateOnly? BirthDate, decimal BonusesAmount);