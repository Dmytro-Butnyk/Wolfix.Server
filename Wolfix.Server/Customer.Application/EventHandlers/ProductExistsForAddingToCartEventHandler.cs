using System.Net;
using Catalog.IntegrationEvents;
using Customer.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

public sealed class ProductExistsForAddingToCartEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<ProductExistsForAddingToCart>
{
    public async Task<VoidResult> HandleAsync(ProductExistsForAddingToCart @event, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(@event.CustomerId, ct);

        if (customer is null)
        {
            return VoidResult.Failure(
                $"Customer with id {@event.CustomerId} not found",
                HttpStatusCode.NotFound
            );
        }

        VoidResult addToCartResult = customer.AddCartItem(
            @event.PhotoUrl,
            @event.Title,
            @event.PriceWithDiscount,
            @event.ProductId
        );
        
        if (!addToCartResult.IsSuccess)
        {
            return addToCartResult;
        }
        
        await customerRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}