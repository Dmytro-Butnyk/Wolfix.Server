using System.Net;
using Catalog.IntegrationEvents;
using Catalog.IntegrationEvents.Dto;
using Seller.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using SellerAggregate = Seller.Domain.SellerAggregate.Seller;

namespace Seller.Application.EventHandlers;

public sealed class FetchSellerInformationEventHandler(
    ISellerRepository sellerRepository)
    : IIntegrationEventHandler<FetchSellerInformation, ProductSellerEventResult>
{
    public async Task<Result<ProductSellerEventResult>> HandleAsync(FetchSellerInformation @event, CancellationToken ct)
    {
        SellerAggregate? seller = await sellerRepository.GetByIdAsNoTrackingAsync(@event.SellerId, ct);

        if (seller is null)
        {
            return Result<ProductSellerEventResult>.Failure($"Seller with id: {@event.SellerId} not found", HttpStatusCode.NotFound);
        }

        var result = new ProductSellerEventResult
        {
            SellerId = seller.Id,
            SellerFullName = seller.GetFullName(),
            SellerPhotoUrl = seller.PhotoUrl
        };
        
        return Result<ProductSellerEventResult>.Success(result);
    }
}