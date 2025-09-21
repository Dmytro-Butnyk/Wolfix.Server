using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record CheckSellerExists(Guid SellerId) : IIntegrationEvent;
