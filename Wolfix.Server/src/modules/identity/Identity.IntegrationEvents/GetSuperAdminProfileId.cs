using Shared.IntegrationEvents.Interfaces;

namespace Identity.IntegrationEvents;

public sealed record GetSuperAdminProfileId : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}