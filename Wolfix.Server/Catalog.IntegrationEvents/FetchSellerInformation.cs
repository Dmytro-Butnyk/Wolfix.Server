using Shared.IntegrationEvents.Interfaces;

namespace Catalog.IntegrationEvents;

public record FetchSellerInformation(Guid SellerId) : IIntegrationEvent;