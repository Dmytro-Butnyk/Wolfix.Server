namespace Catalog.IntegrationEvents.Dto;

public sealed record MediaEventDto(
    string ContentType,
    Stream Filestream);