using Shared.IntegrationEvents.Interfaces;

namespace Seller.IntegrationEvents;

public sealed record CheckCategoryExist : IIntegrationEvent
{
    public required Guid CategoryId { get; init; }
    
    public required string Name { get; init; }
}