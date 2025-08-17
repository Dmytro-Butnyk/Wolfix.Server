using Shared.IntegrationEvents.Inerfaces;

namespace Identity.IntegrationEvents;

public sealed record CustomerAccountCreated : IIntegrationEvent
{
    public Guid AccountId { get; init; }
}