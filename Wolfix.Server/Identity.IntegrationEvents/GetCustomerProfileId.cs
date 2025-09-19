using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record GetCustomerProfileId : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}