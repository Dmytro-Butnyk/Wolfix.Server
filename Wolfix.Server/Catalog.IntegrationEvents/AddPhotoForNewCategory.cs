using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record AddPhotoForNewCategory : IIntegrationEvent
{
    public required Stream Stream { get; init; }
}