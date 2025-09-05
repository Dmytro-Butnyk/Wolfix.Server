using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record ProductExistsForAddingToCart : IIntegrationEvent
{
    public required Guid CustomerId { get; init; }
    
    //todo: добавить фото
    
    public required string Title { get; init; }
    
    public required decimal PriceWithDiscount { get; init; }
}