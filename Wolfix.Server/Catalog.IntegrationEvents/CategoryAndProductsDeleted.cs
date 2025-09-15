using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record CategoryAndProductsDeleted : IIntegrationEvent
{
    public required IReadOnlyCollection<Guid> MediaIds { get; init; }
}