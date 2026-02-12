using System.Net;
using Customer.Domain.Interfaces;
using Order.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

internal sealed class CustomerOrderCreatedEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerOrderCreated>
{
    public async Task<VoidResult> HandleAsync(CustomerOrderCreated @event, CancellationToken ct)
    {
        Domain.CustomerAggregate.Customer? customer = await customerRepository.GetByIdAsync(@event.CustomerId, ct, "_cartItems");

        if (customer is null)
        {
            return VoidResult.Failure(
                $"Customer with id: {@event.CustomerId} not found",
                HttpStatusCode.NotFound
            );
        }

        customer.RemoveAllCartItems();

        await customerRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}