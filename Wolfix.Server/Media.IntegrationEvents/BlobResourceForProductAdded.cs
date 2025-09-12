using Media.IntegrationEvents.Dto;
using Shared.IntegrationEvents.Interfaces;

namespace Media.IntegrationEvents;

public record BlobResourceForProductAdded(
    Guid ProductId,
    BlobResourceAddedDto BlobResource
) : IIntegrationEvent;
