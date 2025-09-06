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
        
        bool isAllSuccess = true;
        
        foreach (var blobResource in @event.BlobResources)
        {
            VoidResult result = product.AddProductMedia(blobResource.Id, blobResource.ContentType, blobResource.Url, blobResource.IsMain);
            
            if (!result.IsSuccess)
            {
                isAllSuccess = false;
            }
        }
        
        await productRepository.SaveChangesAsync(ct);
        
        if (!isAllSuccess)
        {
            return VoidResult.Failure("One or more media files could not be added to the product");
        }
        
        return VoidResult.Success();
    }
}