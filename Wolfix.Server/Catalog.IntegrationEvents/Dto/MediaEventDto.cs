using Shared.Domain.Enums;

namespace Catalog.IntegrationEvents.Dto;

public sealed record MediaEventDto(
    BlobResourceType ContentType,
    Stream Filestream,
    bool IsMain);