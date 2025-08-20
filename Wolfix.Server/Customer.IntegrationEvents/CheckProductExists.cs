using Shared.IntegrationEvents.Interfaces;

namespace Customer.IntegrationEvents;

public sealed record CheckProductExists : IIntegrationEvent
{
    public required Guid ProductId { get; init; }
    
    public required Guid CustomerId { get; init; }
}