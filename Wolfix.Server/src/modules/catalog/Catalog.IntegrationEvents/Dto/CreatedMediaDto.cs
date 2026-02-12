namespace Catalog.IntegrationEvents.Dto;

public sealed record CreatedMediaDto(Guid BlobResourceId, string Url);