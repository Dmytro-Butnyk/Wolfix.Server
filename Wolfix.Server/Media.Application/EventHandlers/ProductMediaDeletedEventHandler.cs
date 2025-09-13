using System.Reflection.Metadata;
using Catalog.IntegrationEvents;
using Media.Application.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class ProductMediaDeletedEventHandler(
    IBlobResourceService blobResourceService)
    : IIntegrationEventHandler<ProductMediaDeleted>
{
    public async Task<VoidResult> HandleAsync(ProductMediaDeleted @event, CancellationToken ct)
    {
        VoidResult deleteBlobResourceResult = await blobResourceService
            .DeleteBlobResourceAsync(@event.MediaId, ct);

        if (!deleteBlobResourceResult.IsSuccess)
        {
            return VoidResult.Failure(deleteBlobResourceResult);
        }

        return VoidResult.Success();
    }
}