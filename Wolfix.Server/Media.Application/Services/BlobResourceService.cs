using Media.Application.Dto;
using Media.Application.Interfaces;
using Media.Application.Options;
using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Media.Application.Services;

public sealed class BlobResourceService(
    IOptionsMonitor<AzureBlobContainersNames> containerNames,
    IBlobResourceRepository blobResourceRepository,
    IAzureBlobRepository azureBlobRepository,
    ILogger<BlobResourceService> logger) : IBlobResourceService
{
    //todo: тут вынести в прайват каррент велью
    
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
        
        blobResourceRepository.Delete(blobResource, ct);
        
        await blobResourceRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }

    public async Task ExecuteDeleteBlobResourceAsync(IReadOnlyCollection<Guid> mediaIds, CancellationToken ct)
    {
        IReadOnlyCollection<BlobResource> medias = await blobResourceRepository.GetAllForDeleteAsync(mediaIds, ct);

        if (medias.Count == 0)
        {
            return;
        }

        foreach (var blobResource in medias)
        {
            string containerName = blobResource.Type switch
            {
                BlobResourceType.Photo => containerNames.CurrentValue.Photos,
                BlobResourceType.Video => containerNames.CurrentValue.Videos,
                //todo: добавить для документов
                _ => throw new Exception($"Unknown blob resource type: {blobResource.Type}")
            };
            
            await TryDeleteBlobFromAzureAsync(containerName, blobResource.Name, ct);
        }
        
        await blobResourceRepository.ExecuteDeleteAsync(media => medias.Contains(media), ct);
    }

    private async Task TryDeleteBlobFromAzureAsync(string containerName, string fileName, CancellationToken ct)
    {
        const uint retryCount = 3;
        uint currentRetryCount = 0;

        while (currentRetryCount < retryCount)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                await azureBlobRepository.DeleteFileAsync(containerName, fileName, ct);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting blob from Azure");
            }
            finally
            {
                ++currentRetryCount;
            }
        }
    }
}