using System.Net;
using Catalog.Domain.Interfaces;
using Catalog.IntegrationEvents;
using Customer.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

public sealed class CheckProductExistsForAddingToCartEventHandler(IProductRepository productRepository, IEventBus eventBus)
    : IIntegrationEventHandler<CheckProductExistsForAddingToCart>
{
    public async Task<VoidResult> HandleAsync(CheckProductExistsForAddingToCart @event, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsNoTrackingAsync(@event.ProductId, ct);

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {@event.ProductId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult result = await eventBus.PublishWithoutResultAsync(new ProductExistsForAddingToCart
        {
            CustomerId = @event.CustomerId,
            PriceWithDiscount = product.FinalPrice,
            Title = product.Title
        }, ct);
        
        return !result.IsSuccess ? result : VoidResult.Success();
    }
}