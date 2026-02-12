using Shared.IntegrationEvents.Interfaces;

namespace Support.IntegrationEvents;

public sealed record CheckCustomerOrder(Guid OrderId, string OrderNumber, Guid CustomerId) : IIntegrationEvent;