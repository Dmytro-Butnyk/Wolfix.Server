using Identity.Application.Interfaces.Repositories;
using Seller.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.EventHandlers;

public sealed class CustomerWantsToBeSellerEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<CustomerWantsToBeSeller>
{
    public async Task<VoidResult> HandleAsync(CustomerWantsToBeSeller @event, CancellationToken ct)
    {
        VoidResult customerExistsAndHasRoleResult = await authStore.CheckUserCanBeSeller(@event.AccountId, ct);

        if (customerExistsAndHasRoleResult.IsFailure)
        {
            return customerExistsAndHasRoleResult;
        }
        
        return VoidResult.Success();
    }
}