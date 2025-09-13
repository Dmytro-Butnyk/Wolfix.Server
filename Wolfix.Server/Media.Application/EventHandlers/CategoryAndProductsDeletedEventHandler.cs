using Catalog.IntegrationEvents;
using Media.Application.Interfaces;
using Media.Application.Options;
using Media.Domain.BlobAggregate;
using Media.Domain.Interfaces;
using Media.Domain.Projections;
using Microsoft.Extensions.Options;
using Shared.Domain.Enums;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class CategoryAndProductsDeletedEventHandler(
    IBlobResourceRepository blobResourceRepository,
    IBlobResourceService blobResourceService)
    : IIntegrationEventHandler<CategoryAndProductsDeleted>
{
    public async Task<VoidResult> HandleAsync(CategoryAndProductsDeleted @event, CancellationToken ct)
    {
        await blobResourceService.ExecuteDeleteBlobResourceAsync(@event.MediaIds, ct);
        
        return VoidResult.Success();
    }
}