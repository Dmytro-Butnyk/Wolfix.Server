using Admin.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Admin.Application.EventHandlers;

public sealed class GetAdminProfileIdEventHandler(IAdminRepository adminRepository)
    : IIntegrationEventHandler<GetAdminProfileId, Guid>
{
    public async Task<Result<Guid>> HandleAsync(GetAdminProfileId @event, CancellationToken ct)
    {
        Guid? adminId = await adminRepository.GetIdByAccountIdAsync(@event.AccountId, ct);

        if (adminId == null)
        {
            return Result<Guid>.Failure($"Admin with account id: {@event.AccountId} not found");
        }
        
        return Result<Guid>.Success(adminId.Value);
    }
}