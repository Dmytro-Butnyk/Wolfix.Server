namespace Support.Application.Dto;

public sealed record CreateSupportRequestDto(Guid CustomerId, string Title, string Category, string Content, Guid? ProductId = null);