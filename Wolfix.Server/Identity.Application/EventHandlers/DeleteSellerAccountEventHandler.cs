using Identity.Application.Interfaces.Repositories;
using Identity.Application.Services;
using Seller.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.EventHandlers;

public sealed class DeleteSellerAccountEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<DeleteSellerAccount>
{
    public async Task<VoidResult> HandleAsync(DeleteSellerAccount @event, CancellationToken ct)
    {
        VoidResult removeSellerRoleResult = await authStore.RemoveSellerRoleAsync(@event.AccountId, ct);

        if (removeSellerRoleResult.IsFailure)
        {
            return removeSellerRoleResult;
        }
        
        return VoidResult.Success();
    }
}