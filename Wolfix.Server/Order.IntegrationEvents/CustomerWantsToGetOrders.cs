using Shared.IntegrationEvents.Interfaces;

namespace Order.IntegrationEvents;

public sealed record CustomerWantsToGetOrders : IIntegrationEvent
{
    public required Guid CustomerId { get; init; }
}