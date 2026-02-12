using System.Net;
using Customer.Domain.Interfaces;
using Order.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

public sealed class CustomerWantsToGetOrdersEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerWantsToGetOrders>
{
    public async Task<VoidResult> HandleAsync(CustomerWantsToGetOrders @event, CancellationToken ct)
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