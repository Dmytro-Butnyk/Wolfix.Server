using System.Net;
using Catalog.Domain.Interfaces;
using Catalog.IntegrationEvents;
using Customer.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

internal sealed class CheckProductExistsEventHandler(IProductRepository productRepository, IEventBus eventBus)
    : IIntegrationEventHandler<CheckProductExists>
{
    public async Task<VoidResult> HandleAsync(CheckProductExists @event, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsNoTrackingAsync(@event.ProductId, ct);

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id {@event.ProductId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult result = await eventBus.PublishAsync(new ProductExistsForAddingToFavorite
        {
            CustomerId = @event.CustomerId,
            Title = product.Title,
            Price = product.Price,
            Bonuses = product.Bonuses,
            AverageRating = product.AverageRating,
            FinalPrice = product.FinalPrice
        }, ct);

        if (!result.IsSuccess)
        {
            return result;
        }
        
        return VoidResult.Success();
    }
}