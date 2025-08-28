namespace Media.IntegrationEvents.Dto;

public record BlobResourceAddedDto(
    string ContentType,
    string Url);