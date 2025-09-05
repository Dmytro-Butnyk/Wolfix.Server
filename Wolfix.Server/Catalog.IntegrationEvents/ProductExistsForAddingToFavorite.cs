using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record ProductExistsForAddingToFavorite : IIntegrationEvent
{
    //todo: фотку добавить
    
    public required Guid CustomerId { get; init; }
    
    public required string Title { get; init; }
    
    public required decimal Price { get; init; }
    
    public required uint Bonuses { get; init; }
    
    public double? AverageRating { get; init; }
    
    public decimal? FinalPrice { get; init; }
}