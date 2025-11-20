using Identity.IntegrationEvents;
using Seller.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.Application.EventHandlers;

internal sealed class GetSellerProfileIdEventHandler(ISellerRepository sellerRepository)
    : IIntegrationEventHandler<GetSellerProfileId, Guid>
{
    public async Task<Result<Guid>> HandleAsync(GetSellerProfileId @event, CancellationToken ct)
    {
        Guid? sellerId = await sellerRepository.GetIdByAccountIdAsync(@event.AccountId, ct);

        if (sellerId == null)
        {
            return Result<Guid>.Failure($"Seller with account id: {@event.AccountId} does not exist");
        }
        
        return Result<Guid>.Success(sellerId.Value);
    }
}