using Shared.IntegrationEvents.Interfaces;

namespace Customer.IntegrationEvents;

public sealed record CheckProductExistsForAddingToCart : IIntegrationEvent
{
    public required Guid ProductId { get; init; }
    
    public required Guid CustomerId { get; init; }
}