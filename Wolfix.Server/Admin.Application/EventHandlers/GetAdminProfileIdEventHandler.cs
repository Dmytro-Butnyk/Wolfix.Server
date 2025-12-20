using System.Net;
using Admin.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Admin.Application.EventHandlers;

internal sealed class GetAdminProfileIdEventHandler(IAdminRepository adminRepository)
    : IIntegrationEventHandler<GetAdminProfileId, Guid>
{
    public async Task<Result<Guid>> HandleAsync(GetAdminProfileId @event, CancellationToken ct)
    {
        Guid? adminId = await adminRepository.GetIdByAccountIdAsync(@event.AccountId, isSuperAdmin: false, ct);

        if (adminId == null)
        {
            return Result<Guid>.Failure(
                $"Basic admin with account id: {@event.AccountId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<Guid>.Success(adminId.Value);
    }
}