using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public sealed record CheckSellerWithCategoryExist(Guid SellerId, Guid CategoryId) : IIntegrationEvent;