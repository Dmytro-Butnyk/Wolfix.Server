using Shared.IntegrationEvents.Interfaces;

namespace Support.IntegrationEvents;

public sealed record CreateSupport : IIntegrationEvent
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}