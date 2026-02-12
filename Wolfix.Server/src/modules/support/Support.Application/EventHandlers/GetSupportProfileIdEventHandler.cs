using System.Net;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.Domain.Interfaces;

namespace Support.Application.EventHandlers;

internal sealed class GetSupportProfileIdEventHandler(ISupportRepository supportRepository)
    : IIntegrationEventHandler<GetSupportProfileId, Guid>
{
    public async Task<Result<Guid>> HandleAsync(GetSupportProfileId @event, CancellationToken ct)
    {
        Guid? supportId = await supportRepository.GetIdByAccountIdAsync(@event.AccountId, ct);
        
        if (supportId == null)
        {
            return Result<Guid>.Failure(
                $"Support with account id: {@event.AccountId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<Guid>.Success(supportId.Value);
    }
}