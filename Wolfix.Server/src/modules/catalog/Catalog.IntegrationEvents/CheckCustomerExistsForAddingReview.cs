using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record CheckCustomerExistsForAddingReview : IIntegrationEvent
{
    public required Guid CustomerId { get; init; }
}