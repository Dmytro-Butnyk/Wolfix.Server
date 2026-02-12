using Shared.IntegrationEvents.Interfaces;

namespace Order.IntegrationEvents;

public sealed record CustomerOrderCreated : IIntegrationEvent
{
    public required List<Guid> CartItemsIds { get; init; }
    
    public required Guid CustomerId { get; init; }
}