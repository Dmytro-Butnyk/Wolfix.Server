using System.Net;
using Catalog.IntegrationEvents;
using Seller.Domain.Interfaces;
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
            .GetByIdAsNoTrackingAsync(@event.SellerId, ct);

        if (seller is null)
        {
            return VoidResult.Failure("Seller not found", HttpStatusCode.NotFound);
        }
        
        return VoidResult.Success();
    }
}