using Catalog.Domain.Interfaces;
using Catalog.Domain.ProductAggregate;
using Media.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

public sealed class BlobResourceForProductAddedEventHandler(IProductRepository productRepository)
    : IIntegrationEventHandler<BlobResourceForProductAdded>
{
    public async Task<VoidResult> HandleAsync(BlobResourceForProductAdded @event, CancellationToken ct)
    {
        Product? product = await productRepository
            .GetByIdAsync(@event.ProductId, ct, "_productMedias");

        if (product is null)
        {
            return VoidResult.Failure("Product not found");
        }

        VoidResult result = product.AddProductMedia(@event.BlobResource.Id, @event.BlobResource.ContentType, @event.BlobResource.Url,
            @event.BlobResource.IsMain);

        if (!result.IsSuccess)
        {
            return VoidResult.Failure("Media file could not be added to the product");
        }

        await productRepository.SaveChangesAsync(ct);

        return VoidResult.Success();
    }
}