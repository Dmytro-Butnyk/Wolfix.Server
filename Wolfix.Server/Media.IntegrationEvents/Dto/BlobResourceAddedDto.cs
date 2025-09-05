using Shared.Domain.Enums;

namespace Media.IntegrationEvents.Dto;

public record BlobResourceAddedDto(
    Guid Id,
    BlobResourceType ContentType,
    string Url);