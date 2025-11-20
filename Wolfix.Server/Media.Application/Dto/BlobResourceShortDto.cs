using Shared.Domain.Enums;

namespace Media.Application.Dto;

public sealed record BlobResourceShortDto
{
    public Guid Id { get; init; }
    public BlobResourceType ContentType { get; init; }
    public string Url { get; init; }
}