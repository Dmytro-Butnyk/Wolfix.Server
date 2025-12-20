using System.Net;
using Customer.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Customer.Application.EventHandlers;

public sealed class CheckCustomerExistsForCreatingSupportRequestEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CheckCustomerExistsForCreatingSupportRequest>
{
    public async Task<VoidResult> HandleAsync(CheckCustomerExistsForCreatingSupportRequest @event, CancellationToken ct)
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