using Shared.IntegrationEvents.Interfaces;

namespace Order.IntegrationEvents;

public sealed record CheckSellerExist(Guid SellerId) : IIntegrationEvent;