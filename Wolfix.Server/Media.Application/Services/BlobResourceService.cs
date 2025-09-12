using Media.Application.Dto;
using Media.Application.Interfaces;
using Media.Application.Options;
using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Media.Application.Services;

public sealed class BlobResourceService(
    IOptionsMonitor<AzureBlobContainersNames> containerNames,
    IBlobResourceRepository blobResourceRepository,
    IAzureBlobRepository azureBlobRepository) : IBlobResourceService
{
    public async Task<Result<BlobResourceShortDto>> AddBlobResourceAsync(BlobResourceType contentType, Stream fileStream, CancellationToken ct)
    {
        Result<BlobResource> blobResource = BlobResource
            .Create(contentType);

        if (!blobResource.IsSuccess)
        {
            return Result<BlobResourceShortDto>.Failure("Invalid blob resource data");
        }

        string url = await azureBlobRepository.AddFileAndGetUrlAsync(
            contentType == BlobResourceType.Photo
                ? containerNames.CurrentValue.Photos
                : containerNames.CurrentValue.Videos,
            blobResource.Value!.Name,fileStream,
            ct);
        
        VoidResult changeUrlResult = blobResource.Value.ChangeUrl(url);

        if (!changeUrlResult.IsSuccess)
        {
            return Result<BlobResourceShortDto>.Failure(changeUrlResult.ErrorMessage!, changeUrlResult.StatusCode);       
        }
        
        await blobResourceRepository.AddAsync(blobResource.Value, ct);
        
        await blobResourceRepository.SaveChangesAsync(ct);
        
        BlobResourceShortDto blobResourceShortDto = new()
        {
            Id = blobResource.Value.Id,
            ContentType = blobResource.Value.Type,
            Url = blobResource.Value.Url
        };
        
        return Result<BlobResourceShortDto>.Success(blobResourceShortDto);
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
        
        blobResourceRepository.DeleteAsync(blobResource, ct);
        
        await blobResourceRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}