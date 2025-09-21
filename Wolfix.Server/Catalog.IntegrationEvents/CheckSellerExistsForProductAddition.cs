using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record CheckSellerExistsForProductAddition(Guid SellerId) : IIntegrationEvent;
