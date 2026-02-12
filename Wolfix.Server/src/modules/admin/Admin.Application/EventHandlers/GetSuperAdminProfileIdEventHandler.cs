using System.Net;
using Admin.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Admin.Application.EventHandlers;

internal sealed class GetSuperAdminProfileIdEventHandler(IAdminRepository adminRepository)
    : IIntegrationEventHandler<GetSuperAdminProfileId, Guid>
{
    public async Task<Result<Guid>> HandleAsync(GetSuperAdminProfileId @event, CancellationToken ct)
    {
        Guid? adminId = await adminRepository.GetIdByAccountIdAsync(@event.AccountId, isSuperAdmin: true, ct);

        if (adminId == null)
        {
            return Result<Guid>.Failure(
                $"Super admin with account id: {@event.AccountId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<Guid>.Success(adminId.Value);
    }
}