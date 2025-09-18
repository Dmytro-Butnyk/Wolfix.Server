using Catalog.Domain.Interfaces;
using Order.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

internal sealed class CustomerWantsToPlaceOrderItemsEventHandler(IProductRepository productRepository)
    : IIntegrationEventHandler<CustomerWantsToPlaceOrderItems>
{
    public async Task<VoidResult> HandleAsync(CustomerWantsToPlaceOrderItems @event, CancellationToken ct)
    {
        throw new Exception();
    }
}