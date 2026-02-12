using Shared.IntegrationEvents.Interfaces;

namespace Seller.IntegrationEvents;

public sealed record DeleteSellerAccount : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}