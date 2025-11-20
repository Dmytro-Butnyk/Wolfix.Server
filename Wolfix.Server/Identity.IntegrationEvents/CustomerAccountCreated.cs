using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record CustomerAccountCreated : IIntegrationEvent
{
    public Guid AccountId { get; init; }
}