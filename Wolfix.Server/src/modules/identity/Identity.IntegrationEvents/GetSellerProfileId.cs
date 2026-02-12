using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record GetSellerProfileId : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}