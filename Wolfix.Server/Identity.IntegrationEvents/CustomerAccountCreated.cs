using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record CustomerAccountCreated : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}