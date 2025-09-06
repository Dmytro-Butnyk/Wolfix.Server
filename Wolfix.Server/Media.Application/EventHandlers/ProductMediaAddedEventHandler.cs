using Catalog.IntegrationEvents;
using Media.Application.Dto;
using Media.Application.Interfaces;
using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Media.IntegrationEvents;
using Media.IntegrationEvents.Dto;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class ProductMediaAddedEventHandler(
    IBlobResourceService blobResourceService,
    IEventBus eventBus)
    : IIntegrationEventHandler<ProductMediaAdded>
{
    public async Task<VoidResult> HandleAsync(ProductMediaAdded @event, CancellationToken ct)
    {
        List<BlobResourceAddedDto> blobResources = new(@event.Medias.Count);

        bool isAllSuccess = true;
        foreach (var mediaEventDto in @event.Medias)
        {
            Result<BlobResourceShortDto> result = await blobResourceService
                    .AddBlobResourceAsync(mediaEventDto.ContentType, mediaEventDto.Filestream, ct);

            if (!result.IsSuccess)
            {
                isAllSuccess = false;
                continue;
            }
            
            blobResources.Add(new BlobResourceAddedDto(result.Value.Id, result.Value.ContentType, result.Value.Url, mediaEventDto.IsMain));
        }

        if (!isAllSuccess)
        {
            return VoidResult.Failure("One or more media files could not be processed");
        }
        
        VoidResult publishResult = await eventBus
            .PublishAsync(new BlobResourcesForProductAdded(@event.ProductId, blobResources), ct); 
        
        if (!publishResult.IsSuccess)
        {
            return VoidResult.Failure(publishResult.ErrorMessage!, publishResult.StatusCode);
        }
        
        return VoidResult.Success();
    }
}