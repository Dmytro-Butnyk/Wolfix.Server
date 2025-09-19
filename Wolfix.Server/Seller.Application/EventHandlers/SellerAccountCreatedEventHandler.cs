using Identity.IntegrationEvents;
using Seller.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using SellerAggregate = Seller.Domain.SellerAggregate.Seller;

namespace Seller.Application.EventHandlers;

public sealed class SellerAccountCreatedEventHandler(ISellerRepository sellerRepository)
    : IIntegrationEventHandler<SellerAccountCreated, Guid>
{
    public async Task<Result<Guid>> HandleAsync(SellerAccountCreated @event, CancellationToken ct)
    {
        Result<SellerAggregate> createSellerResult =
            SellerAggregate.Create(@event.AccountId, @event.FirstName, @event.LastName, @event.MiddleName,
                @event.PhoneNumber, @event.City, @event.Street, @event.HouseNumber, @event.ApartmentNumber, @event.BirthDate);
        
        if (createSellerResult.IsFailure)
        {
            return Result<Guid>.Failure(createSellerResult);
        }
        
        SellerAggregate seller = createSellerResult.Value!;
        
        await sellerRepository.AddAsync(seller, ct);
        
        await sellerRepository.SaveChangesAsync(ct);
        
        return Result<Guid>.Success(seller.Id);
    }
}