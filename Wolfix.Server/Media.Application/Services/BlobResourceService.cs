using Media.Application.Interfaces;
using Media.Application.Options;
using Media.Domain.BlobAggregate;
using Media.Domain.BlobAggregate.Enums;
using Media.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Domain.Models;

namespace Media.Application.Services;

public sealed class BlobResourceService(
    IOptionsMonitor<AzureBlobContainersNames> containerNames,
    IBlobResourceRepository blobResourceRepository,
    IAzureBlobRepository azureBlobRepository) : IBlobResourceService
{
    public async Task<VoidResult> AddBlobResourceAsync(string contentType, Stream fileStream, CancellationToken ct)
    {

        BlobResourceType blobResourceType = Enum.Parse<BlobResourceType>(contentType);
        Result<BlobResource> blobResource = BlobResource
            .Create(blobResourceType);

        if (!blobResource.IsSuccess)
        {
            return VoidResult.Failure("Invalid blob resource data");
        }

        string url = await azureBlobRepository.AddFileAndGetUrlAsync(
            blobResourceType == BlobResourceType.Photo
                ? containerNames.CurrentValue.Photos
                : containerNames.CurrentValue.Videos,
            blobResource.Value!.Name,fileStream,
            ct);
        
        blobResource.Value.ChangeUrl(url);
        
        await blobResourceRepository.AddAsync(blobResource.Value, ct);
        
        return VoidResult.Success();
    }

    public async Task<VoidResult> DeleteBlobResourceAsync(Guid id, CancellationToken ct)
    {
        BlobResource? blobResource = await blobResourceRepository.GetByIdAsync(id, ct);
        
        if (blobResource is null)
        {
            return VoidResult.Failure("Blob resource not found");
        }
        
        string containerName = blobResource.Type == BlobResourceType.Photo
            ? containerNames.CurrentValue.Photos
            : containerNames.CurrentValue.Videos;
        
        string fileName = blobResource.Name;
        
        await azureBlobRepository.DeleteFileAsync(containerName, fileName, ct);
        
        await blobResourceRepository.DeleteAsync(blobResource, ct);
        
        return VoidResult.Success();
    }
}