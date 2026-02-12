using Admin.IntegrationEvents;
using Identity.Application.Interfaces.Repositories;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.EventHandlers;

public sealed class DeleteAdminAccountEventHandler(IAuthStore authStore)
    : IIntegrationEventHandler<DeleteAdminAccount>
{
    public async Task<VoidResult> HandleAsync(DeleteAdminAccount @event, CancellationToken ct) 
        => await authStore.RemoveRoleOrWholeAccountAsync(@event.AccountId, Roles.Admin, ct);
}