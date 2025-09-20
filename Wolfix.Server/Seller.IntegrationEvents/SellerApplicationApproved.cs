using Shared.IntegrationEvents.Interfaces;

namespace Seller.IntegrationEvents;

public sealed record SellerApplicationApproved : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}