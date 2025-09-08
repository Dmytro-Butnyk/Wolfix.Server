using Catalog.IntegrationEvents.Dto;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record ProductMediaAdded : IIntegrationEvent
{
    public Guid ProductId { get; init; }
    public MediaEventDto Media { get; init; } = null!;
}