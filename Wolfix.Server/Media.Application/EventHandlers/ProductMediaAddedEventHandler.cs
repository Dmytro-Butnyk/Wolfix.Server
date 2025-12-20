using Catalog.IntegrationEvents;
using Media.Application.Dto;
using Media.Application.Services;
using Media.IntegrationEvents;
using Media.IntegrationEvents.Dto;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class ProductMediaAddedEventHandler(
    BlobResourceService blobResourceService,
    EventBus eventBus)
    : IIntegrationEventHandler<ProductMediaAdded>
{
    public async Task<VoidResult> HandleAsync(ProductMediaAdded @event, CancellationToken ct)
    {
        Result<BlobResourceShortDto> result = await blobResourceService
            .AddBlobResourceAsync(@event.Media.ContentType, @event.Media.FileData, ct);

        if (!result.IsSuccess)
        {
            return VoidResult.Failure(result.ErrorMessage!, result.StatusCode);
        }

        BlobResourceAddedDto blobResourceAddedDto =
            new(result.Value!.Id, result.Value.ContentType, result.Value.Url, @event.Media.IsMain);

        VoidResult publishResult = await eventBus
            .PublishWithoutResultAsync(new BlobResourceForProductAdded(@event.ProductId, blobResourceAddedDto), ct);

        if (!publishResult.IsSuccess)
        {
            return VoidResult.Failure(publishResult.ErrorMessage!, publishResult.StatusCode);
        }

        return VoidResult.Success();
    }
}