using Azure.Storage.Blobs;
using Media.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Media.Infrastructure.Repositories;

internal sealed class AzureBlobRepository : IAzureBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobRepository(IConfiguration configuration)
    {
        string connectionString = configuration["BLOB"]
                                  ?? throw new Exception("Configuration key BLOB not found");
        
        _blobServiceClient = new BlobServiceClient(connectionString);
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