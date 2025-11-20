using Shared.IntegrationEvents.Interfaces;

namespace Admin.IntegrationEvents;

public sealed record CreateAdmin : IIntegrationEvent
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}