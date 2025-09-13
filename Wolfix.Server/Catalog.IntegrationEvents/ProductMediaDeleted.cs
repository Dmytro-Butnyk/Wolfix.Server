using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record ProductMediaDeleted(Guid MediaId) : IIntegrationEvent;
