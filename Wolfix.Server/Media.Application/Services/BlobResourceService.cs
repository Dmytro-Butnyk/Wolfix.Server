using Media.Application.Dto;
using Media.Application.Options;
using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Domain.Enums;
using Shared.Domain.Models;

namespace Media.Application.Services;

public sealed class BlobResourceService(
    IOptionsMonitor<AzureBlobContainersNames> containerNames,
    IBlobResourceRepository blobResourceRepository,
    IAzureBlobRepository azureBlobRepository,
    ILogger<BlobResourceService> logger)
{
    private readonly AzureBlobContainersNames _containerNames = containerNames.CurrentValue;
    
    public async Task<Result<BlobResourceShortDto>> AddBlobResourceAsync(BlobResourceType contentType, IFormFile fileData, CancellationToken ct)
    {
        Result<BlobResource> createBlobResourceResult = BlobResource.Create(contentType);

        if (!createBlobResourceResult.IsSuccess)
        {
            return Result<BlobResourceShortDto>.Failure(createBlobResourceResult);
        }

        string containerName = contentType switch
        {
            BlobResourceType.Photo => _containerNames.Photos,
            BlobResourceType.Video => _containerNames.Videos,
            BlobResourceType.Document => _containerNames.Documents,
            _ => throw new Exception($"{nameof(AddBlobResourceAsync)} -> Unknown blob resource type: {contentType}")
        };
        
        await using Stream fileStream = fileData.OpenReadStream();
        
        string extension = Path.GetExtension(fileData.FileName);
        string fileName = $"{createBlobResourceResult.Value!.Name}{extension}";

        string url = await azureBlobRepository.AddFileAndGetUrlAsync(
            containerName,
            fileName, fileStream,
            ct);
        
        VoidResult changeUrlResult = createBlobResourceResult.Value.ChangeUrl(url);

        if (!changeUrlResult.IsSuccess)
        {
            return Result<BlobResourceShortDto>.Failure(changeUrlResult.ErrorMessage!, changeUrlResult.StatusCode);       
        }
        
        await blobResourceRepository.AddAsync(createBlobResourceResult.Value, ct);
        
        await blobResourceRepository.SaveChangesAsync(ct);
        
        BlobResourceShortDto blobResourceShortDto = new()
        {
            Id = createBlobResourceResult.Value.Id,
            ContentType = createBlobResourceResult.Value.Type,
            Url = createBlobResourceResult.Value.Url
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
        
        string containerName = blobResource.Type switch
        {
            BlobResourceType.Photo => _containerNames.Photos,
            BlobResourceType.Video => _containerNames.Videos,
            BlobResourceType.Document => _containerNames.Documents,
            _ => throw new Exception($"{nameof(DeleteBlobResourceAsync)} -> Unknown blob resource type: {blobResource.Type}")
        };
        
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
                BlobResourceType.Photo => _containerNames.Photos,
                BlobResourceType.Video => _containerNames.Videos,
                BlobResourceType.Document => _containerNames.Documents,
                _ => throw new Exception($"{nameof(ExecuteDeleteBlobResourceAsync)} -> Unknown blob resource type: {blobResource.Type}")
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