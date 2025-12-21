using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record DeleteProductMedia : IIntegrationEvent
{
    public required string? MediaUrl { get; init; }
}