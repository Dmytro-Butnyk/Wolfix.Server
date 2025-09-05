using System.Net;
using Catalog.IntegrationEvents;
using Customer.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

internal sealed class ProductExistsForAddingToFavoriteEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<ProductExistsForAddingToFavorite>
{
    public async Task<VoidResult> HandleAsync(ProductExistsForAddingToFavorite @event, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(@event.CustomerId, ct);

        if (customer is null)
        {
            return VoidResult.Failure(
                $"Customer with id {@event.CustomerId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult addToFavoriteResult = customer.AddFavoriteItem(
            @event.Title,
            "//todo: add image url",
            @event.Price,
            @event.Bonuses,
            @event.AverageRating,
            @event.FinalPrice
        );
        
        if (!addToFavoriteResult.IsSuccess)
        {
            return addToFavoriteResult;
        }

        await customerRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}