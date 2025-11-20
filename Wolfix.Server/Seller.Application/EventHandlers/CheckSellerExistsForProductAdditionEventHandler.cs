using System.Net;
using Catalog.IntegrationEvents;
using Seller.Domain.Interfaces;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using SellerAggregate = Seller.Domain.SellerAggregate.Seller;

namespace Seller.Application.EventHandlers;

public sealed class CheckSellerExistsForProductAdditionEventHandler(
    ISellerRepository sellerRepository)
    : IIntegrationEventHandler<CheckSellerExistsForProductAddition>
{
    public async Task<VoidResult> HandleAsync(CheckSellerExistsForProductAddition @event, CancellationToken ct)
    {
        SellerAggregate? seller = await sellerRepository
            .GetByIdAsNoTrackingAsync(@event.SellerId, ct, "_sellerCategories");

        if (seller is null)
        {
            return VoidResult.Failure("Seller not found", HttpStatusCode.NotFound);
        }

        SellerCategoryInfo? sellerCategory = seller.SellerCategories.FirstOrDefault(sc => sc.CategoryId == @event.CategoryId);

        if (sellerCategory is null)
        {
            return VoidResult.Failure($"Seller does not have category with id: {@event.CategoryId}");
        }
        
        return VoidResult.Success();
    }
}