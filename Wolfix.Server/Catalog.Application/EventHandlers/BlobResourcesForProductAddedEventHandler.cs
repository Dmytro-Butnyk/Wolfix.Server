using Catalog.Domain.Interfaces;
using Catalog.Domain.ProductAggregate;
using Media.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

public sealed class BlobResourcesForProductAddedEventHandler(
    IProductRepository productRepository
    ) : IIntegrationEventHandler<BlobResourcesForProductAdded>
{
    public async Task<VoidResult> HandleAsync(BlobResourcesForProductAdded @event, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(@event.ProductId, ct);
        
        if (product is null)
        {
            return VoidResult.Failure("Product not found");
        }
        
        foreach (var blobResource in @event.BlobResources)
        {
            product.AddProductMedia(blobResource.Id, blobResource.ContentType, blobResource.Url);
        }
        
        //TODO: Handle possible concurrency issues
        // await productRepository.SaveChangesAsync(ct);
        
    }
}