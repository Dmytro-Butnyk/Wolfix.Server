using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record GetAdminProfileId : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}