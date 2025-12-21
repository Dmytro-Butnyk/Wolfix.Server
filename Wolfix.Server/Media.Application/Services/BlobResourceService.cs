using System.Net;
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
        
        await using Stream fileStream = fileData.OpenReadStream();
        
        string extension = Path.GetExtension(fileData.FileName);
        string fileName = $"{createBlobResourceResult.Value!.Name}{extension}";

        string url = await azureBlobRepository.AddFileAndGetUrlAsync(
            GetContainerName(createBlobResourceResult.Value!.Type),
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
        
        string fileName = blobResource.Name;
        
        await azureBlobRepository.DeleteFileAsync(GetContainerName(blobResource.Type), fileName, ct);
        
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
            await TryDeleteBlobFromAzureAsync(GetContainerName(blobResource.Type), blobResource.Name, ct);
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

    public async Task<VoidResult> DeleteByUrlAsync(string mediaUrl, CancellationToken ct)
    {
        BlobResource? blobResource = await blobResourceRepository.GetByUrlAsync(mediaUrl, ct);

        if (blobResource is null)
        {
            return VoidResult.Failure(
                $"Blob resource with url: {mediaUrl} not found",
                HttpStatusCode.NotFound
            );
        }
        
        VoidResult deleteFromDbResult = await DeleteBlobResourceAsync(blobResource.Id, ct);

        if (deleteFromDbResult.IsFailure)
        {
            return deleteFromDbResult;
        }

        await TryDeleteBlobFromAzureAsync(GetContainerName(blobResource.Type), blobResource.Name, ct);
        
        return VoidResult.Success();
    }
    
    private string GetContainerName(BlobResourceType type)
        => type switch
        {
            BlobResourceType.Photo => _containerNames.Photos,
            BlobResourceType.Video => _containerNames.Videos,
            BlobResourceType.Document => _containerNames.Documents,
            _ => throw new Exception($"Unknown blob resource type: {type}")
        };
}