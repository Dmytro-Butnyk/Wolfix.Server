using Catalog.IntegrationEvents.Dto;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record ProductMediaAdded(Guid ProductId, MediaEventDto Media) : IIntegrationEvent;
