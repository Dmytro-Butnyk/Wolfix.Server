using System.Net;
using Catalog.Domain.Interfaces;
using Catalog.IntegrationEvents;
using Customer.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

internal sealed class CheckProductExistsForAddingToFavoriteEventHandler(IProductRepository productRepository, EventBus eventBus)
    : IIntegrationEventHandler<CheckProductExistsForAddingToFavorite>
{
    public async Task<VoidResult> HandleAsync(CheckProductExistsForAddingToFavorite @event, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsNoTrackingAsync(@event.ProductId, ct, "_productMedias");

        if (product is null)
        {
            return VoidResult.Failure(
                $"Product with id: {@event.ProductId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult result = await eventBus.PublishWithoutResultAsync(new ProductExistsForAddingToFavorite
        {
            PhotoUrl = product.MainPhotoUrl!,
            CustomerId = @event.CustomerId,
            Title = product.Title,
            Price = product.Price,
            Bonuses = product.Bonuses,
            AverageRating = product.AverageRating,
            FinalPrice = product.FinalPrice
        }, ct);

        return !result.IsSuccess ? result : VoidResult.Success();
    }
}