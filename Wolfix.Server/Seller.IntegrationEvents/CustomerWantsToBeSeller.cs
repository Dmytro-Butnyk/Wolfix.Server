using Shared.IntegrationEvents.Interfaces;

namespace Seller.IntegrationEvents;

public sealed record CustomerWantsToBeSeller : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}