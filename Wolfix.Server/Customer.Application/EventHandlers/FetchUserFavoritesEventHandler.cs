using System.Net;
using Customer.Domain.Interfaces;
using Customer.IntegrationEvents;
using Notification.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using CustomerAggregate = Customer.Domain.CustomerAggregate.Customer;

namespace Customer.Application.EventHandlers;

public sealed class FetchUserFavoritesEventHandler(
    ICustomerRepository customerRepository,
    IEventBus eventBus)
    : IIntegrationEventHandler<FetchUserFavorites>
{
    public async Task<VoidResult> HandleAsync(FetchUserFavorites @event, CancellationToken ct)
    {
        CustomerAggregate? customer =
            await customerRepository.GetByIdAsNoTrackingAsync(
                @event.UserId,
                ct,
                "_favoriteItems");

        if (customer is null)
        {
            return VoidResult.Failure(
                $"Customer id: {@event.UserId} not found",
                HttpStatusCode.NotFound);
        }
        
        IReadOnlyCollection<Guid> favoriteItems = customer.FavoriteItems
            .Select(x => x.Id)
            .ToList();
        
        VoidResult publishFavoriteItemsResult =
            await eventBus.PublishWithoutResultAsync(new FetchedUserFavorites(@event.UserId, favoriteItems, @event.ConnectionId), ct);

        if (publishFavoriteItemsResult.IsFailure)
        {
            return VoidResult.Failure(publishFavoriteItemsResult);       
        }
        
        return VoidResult.Success();
    }
}