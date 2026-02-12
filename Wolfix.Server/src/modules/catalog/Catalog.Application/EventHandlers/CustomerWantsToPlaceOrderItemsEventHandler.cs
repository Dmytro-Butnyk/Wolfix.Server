using System.Net;
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
        int existingCount = await productRepository.GetTotalCountByIdsAsync(@event.ProductIds, ct);
        
        bool allExist = existingCount == @event.ProductIds.Count;

        if (!allExist)
        {
            return VoidResult.Failure(
                "Not all products exist for placing order",
                HttpStatusCode.NotFound
            );
        }
        
        return VoidResult.Success();
    }
}