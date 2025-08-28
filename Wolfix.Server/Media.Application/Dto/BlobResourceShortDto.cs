namespace Media.Application.Dto;

public sealed record BlobResourceShortDto
{
    public string ContentType { get; init; }
    public string Url { get; init; }
}