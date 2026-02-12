using Microsoft.AspNetCore.Http;
using Shared.Domain.Enums;

namespace Catalog.IntegrationEvents.Dto;

public sealed record MediaEventDto(
    BlobResourceType ContentType,
    IFormFile FileData,
    bool IsMain);