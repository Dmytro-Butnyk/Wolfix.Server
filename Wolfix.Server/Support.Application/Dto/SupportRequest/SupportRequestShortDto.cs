namespace Support.Application.Dto.SupportRequest;

public sealed record SupportRequestShortDto(Guid Id, string Category, string RequestContent, DateTime CreatedAt);