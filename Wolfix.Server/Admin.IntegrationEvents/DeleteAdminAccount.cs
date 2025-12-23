using Shared.IntegrationEvents.Interfaces;

namespace Admin.IntegrationEvents;

public sealed record DeleteAdminAccount : IIntegrationEvent
{
    public required Guid AccountId { get; init; }
}