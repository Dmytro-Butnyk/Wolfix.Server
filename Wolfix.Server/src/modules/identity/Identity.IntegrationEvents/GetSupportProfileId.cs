using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record GetSupportProfileId : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}