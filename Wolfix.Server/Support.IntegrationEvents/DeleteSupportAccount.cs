using Shared.IntegrationEvents.Interfaces;

namespace Support.IntegrationEvents;

public sealed record DeleteSupportAccount : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}