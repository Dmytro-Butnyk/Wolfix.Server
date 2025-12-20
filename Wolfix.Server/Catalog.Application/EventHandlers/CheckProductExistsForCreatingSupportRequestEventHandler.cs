using System.Net;
using Catalog.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Catalog.Application.EventHandlers;

public sealed class CheckProductExistsForCreatingSupportRequestEventHandler(IProductRepository productRepository)
    : IIntegrationEventHandler<CheckProductExistsForCreatingSupportRequest>
{
    public async Task<VoidResult> HandleAsync(CheckProductExistsForCreatingSupportRequest @event, CancellationToken ct)
    {
        if (!await productRepository.IsExistAsync(@event.ProductId, ct))
        {
            return VoidResult.Failure(
                $"Product with id: {@event.ProductId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return VoidResult.Success();
    }
}