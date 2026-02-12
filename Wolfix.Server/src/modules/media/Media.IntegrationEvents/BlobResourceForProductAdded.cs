using Media.IntegrationEvents.Dto;
using Shared.IntegrationEvents.Interfaces;

namespace Media.IntegrationEvents;

public sealed record BlobResourceForProductAdded(
    Guid ProductId,
    BlobResourceAddedDto BlobResource
) : IIntegrationEvent;
