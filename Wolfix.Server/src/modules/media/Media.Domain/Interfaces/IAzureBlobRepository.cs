namespace Media.Domain.Interfaces;

public interface IAzureBlobRepository
{
    Task<string> AddFileAndGetUrlAsync(string containerName, string fileName, Stream fileStream, CancellationToken ct);
    string GetFileUrl(string containerName, string fileName, CancellationToken ct);
    Task DeleteFileAsync(string containerName, string fileName, CancellationToken ct);
}