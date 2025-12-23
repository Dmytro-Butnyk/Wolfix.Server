using Shared.Domain.ValueObjects;

namespace Support.Application.Dto;

public sealed record SupportRequestShortDto(Guid Id, string Category, string RequestContent, DateTime CreatedAt);