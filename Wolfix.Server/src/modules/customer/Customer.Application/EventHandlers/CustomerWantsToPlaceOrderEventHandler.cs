using System.Net;
using Customer.Domain.Interfaces;
using Order.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

internal sealed class CustomerWantsToPlaceOrderEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerWantsToPlaceOrder>
{
    public async Task<VoidResult> HandleAsync(CustomerWantsToPlaceOrder @event, CancellationToken ct)
    {
        if (!await customerRepository.IsExistAsync(@event.CustomerId, ct))
        {
            return VoidResult.Failure(
                $"Customer with id: {@event.CustomerId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return VoidResult.Success();
    }
}