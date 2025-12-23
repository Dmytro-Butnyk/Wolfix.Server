namespace Support.Application.Dto;

public sealed record CreateSupportRequestDto(
    Guid CustomerId,
    string Category,
    string Content);