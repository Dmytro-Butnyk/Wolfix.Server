using Catalog.IntegrationEvents;
using Media.Application.Services;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class CategoryAndProductsDeletedEventHandler(BlobResourceService blobResourceService)
    : IIntegrationEventHandler<CategoryAndProductsDeleted>
{
    public async Task<VoidResult> HandleAsync(CategoryAndProductsDeleted @event, CancellationToken ct)
    {
        await blobResourceService.ExecuteDeleteBlobResourceAsync(@event.MediaIds, ct);
        
        return VoidResult.Success();
    }
}