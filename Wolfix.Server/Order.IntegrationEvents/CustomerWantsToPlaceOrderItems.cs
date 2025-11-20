using Shared.IntegrationEvents.Interfaces;

namespace Order.IntegrationEvents;

public sealed record CustomerWantsToPlaceOrderItems : IIntegrationEvent
{
    public required IReadOnlyCollection<Guid> ProductIds { get; init; }
}