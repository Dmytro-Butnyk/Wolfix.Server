using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record ProductExistsForAddingToCart : IIntegrationEvent
{
    public required Guid CustomerId { get; init; }
    
    public required string PhotoUrl { get; init; }
    
    public required string Title { get; init; }
    
    public required decimal PriceWithDiscount { get; init; }
    
    public required Guid ProductId { get; init; }
}