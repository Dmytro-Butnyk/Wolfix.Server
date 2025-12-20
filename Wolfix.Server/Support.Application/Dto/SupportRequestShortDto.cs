using Shared.Domain.ValueObjects;

namespace Support.Application.Dto;

public sealed record SupportRequestShortDto(Guid Id, Email Email, FullName FullName, PhoneNumber PhoneNumber,
    string Title, DateTime CreatedAt);