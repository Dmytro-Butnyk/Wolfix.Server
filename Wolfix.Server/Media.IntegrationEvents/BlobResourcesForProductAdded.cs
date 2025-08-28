using Media.IntegrationEvents.Dto;
using Shared.IntegrationEvents.Interfaces;

namespace Media.IntegrationEvents;

public record BlobResourcesForProductAdded(
    Guid ProductId,
    IReadOnlyCollection<BlobResourceAddedDto> BlobResources
) : IIntegrationEvent;
