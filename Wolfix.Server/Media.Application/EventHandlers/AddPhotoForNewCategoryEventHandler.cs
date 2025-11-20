using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Media.Application.Dto;
using Media.Application.Interfaces;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class AddPhotoForNewCategoryEventHandler(IBlobResourceService blobResourceService)
    : IIntegrationEventHandler<AddPhotoForNewCategory, CreatedMediaDto>
{
    public async Task<Result<CreatedMediaDto>> HandleAsync(AddPhotoForNewCategory @event, CancellationToken ct)
    {
        Result<BlobResourceShortDto> createBlobResourceResult =
            await blobResourceService.AddBlobResourceAsync(BlobResourceType.Photo, @event.FileData, ct);

        if (createBlobResourceResult.IsFailure)
        {
            return Result<CreatedMediaDto>.Failure(createBlobResourceResult);
        }

        BlobResourceShortDto blobResource = createBlobResourceResult.Value!;
        
        return Result<CreatedMediaDto>.Success(new(blobResource.Id, blobResource.Url));
    }
}