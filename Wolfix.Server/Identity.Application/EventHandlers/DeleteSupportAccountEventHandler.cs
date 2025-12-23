using Identity.Application.Interfaces.Repositories;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Identity.Application.EventHandlers;

public sealed class DeleteSupportAccountEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<DeleteSupportAccount>
{
    public async Task<VoidResult> HandleAsync(DeleteSupportAccount @event, CancellationToken ct)
    {
        VoidResult removeRoleOrAccountResult = await authStore.RemoveSupportRoleOrWholeAccountAsync(@event.AccountId, ct);

        if (removeRoleOrAccountResult.IsFailure)
        {
            return removeRoleOrAccountResult;
        }
        
        return VoidResult.Success();
    }
}