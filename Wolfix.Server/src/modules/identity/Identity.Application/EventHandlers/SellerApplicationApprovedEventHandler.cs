using Identity.Application.Interfaces.Repositories;
using Seller.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.EventHandlers;

public sealed class SellerApplicationApprovedEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<SellerApplicationApproved>
{
    public async Task<VoidResult> HandleAsync(SellerApplicationApproved @event, CancellationToken ct)
    {
        VoidResult addSellerRoleToCustomer = await authStore.AddSellerRoleAsync(@event.AccountId, ct);

        if (addSellerRoleToCustomer.IsFailure)
        {
            return addSellerRoleToCustomer;
        }
        
        return VoidResult.Success();
    }
}