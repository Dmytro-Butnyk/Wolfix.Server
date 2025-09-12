using Identity.IntegrationEvents;
using Seller.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.Application.EventHandlers;

public sealed class SellerAccountCreatedEventHandler(ISellerRepository sellerRepository)
    : IIntegrationEventHandler<SellerAccountCreated>
{
    public async Task<VoidResult> HandleAsync(SellerAccountCreated @event, CancellationToken ct)
    {
        Result<Domain.SellerAggregate.Seller> createSellerResult =
            Domain.SellerAggregate.Seller.Create(@event.AccountId, @event.FirstName, @event.LastName, @event.MiddleName,
                @event.PhoneNumber, @event.City, @event.Street, @event.HouseNumber, @event.ApartmentNumber, @event.BirthDate);
        
        if (!createSellerResult.IsSuccess)
        {
            return VoidResult.Failure(createSellerResult.ErrorMessage!, createSellerResult.StatusCode);
        }
        
        await sellerRepository.AddAsync(createSellerResult.Value!, ct);
        
        await sellerRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}