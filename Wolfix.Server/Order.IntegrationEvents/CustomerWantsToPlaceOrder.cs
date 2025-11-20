using Shared.IntegrationEvents.Interfaces;

namespace Order.IntegrationEvents;

public sealed record CustomerWantsToPlaceOrder : IIntegrationEvent
{
    public required Guid CustomerId { get; init; }
}