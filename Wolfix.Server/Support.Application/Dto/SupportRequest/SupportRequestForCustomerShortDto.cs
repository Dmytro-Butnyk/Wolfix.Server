using Support.Domain.Enums;

namespace Support.Application.Dto.SupportRequest;

public sealed record SupportRequestForCustomerShortDto(Guid Id, string Category, string RequestContent, string Status, DateTime CreatedAt);