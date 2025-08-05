using Azure.Storage.Blobs;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Repositories;

public sealed class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobRepository()
    {
        string? connectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING");
        
        _blobServiceClient = new BlobServiceClient(connectionString!);
    }
    
    public async Task<string> AddFileAndGetUrlAsync(string containerName, string fileName, Stream fileStream, CancellationToken ct)
    {
        BlobContainerClient? container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);
        
        BlobClient? blobClient = container.GetBlobClient(fileName);
        
        await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken: ct);

        await fileStream.DisposeAsync();
        
        return blobClient.Uri.ToString();
    }

    public string GetFileUrl(string containerName, string fileName, CancellationToken ct)
    {
        BlobContainerClient? blobClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient? blob = blobClient.GetBlobClient(fileName);
        
        return blob.Uri.ToString();
    }

    public async Task DeleteFileAsync(string containerName, string fileName, CancellationToken ct)
    {
        BlobContainerClient? container = _blobServiceClient.GetBlobContainerClient(containerName);
        
        BlobClient? blobClient = container.GetBlobClient(fileName);
        
        await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
    }
}