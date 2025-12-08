using System.Net;
using Catalog.IntegrationEvents;
using Order.IntegrationEvents;
using Seller.Domain.Interfaces;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Seller.Application.EventHandlers;

public sealed class CheckSellerWithCategoryExistEventHandler(ISellerRepository sellerRepository) : IIntegrationEventHandler<CheckSellerWithCategoryExist>
{
    public async Task<VoidResult> HandleAsync(CheckSellerWithCategoryExist @event, CancellationToken ct)
    {
        Domain.SellerAggregate.Seller? seller =
            await sellerRepository.GetByIdAsNoTrackingAsync(@event.SellerId, ct, "_sellerCategories");

        if (seller is null)
        {
            return VoidResult.Failure(
                $"Seller with id: {@event.SellerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        bool sellerDoesntOwnThisCategory = seller.SellerCategories.FirstOrDefault(sc => sc.CategoryId == @event.CategoryId) is null;

        if (sellerDoesntOwnThisCategory)
        {
            return VoidResult.Failure(
                $"Seller does not have category with id: {@event.CategoryId}",
                HttpStatusCode.NotFound
            );
        }
        
        return VoidResult.Success();
    }
}