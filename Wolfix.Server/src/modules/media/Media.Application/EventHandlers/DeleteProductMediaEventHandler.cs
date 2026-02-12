using System.Net;
using Catalog.IntegrationEvents;
using Media.Application.Services;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Media.Application.EventHandlers;

public sealed class DeleteProductMediaEventHandler(BlobResourceService blobResourceService)
    : IIntegrationEventHandler<DeleteProductMedia>
{
    public async Task<VoidResult> HandleAsync(DeleteProductMedia @event, CancellationToken ct)
        => @event.MediaUrl == null ? VoidResult.Failure("Media url is null")
            : await blobResourceService.DeleteByUrlAsync(@event.MediaUrl, ct);
}