using System.Net;
using Customer.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

internal sealed class GetCustomerProfileIdEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<GetCustomerProfileId, Guid>
{
    public async Task<Result<Guid>> HandleAsync(GetCustomerProfileId @event, CancellationToken ct)
    {
        Guid? customerId = await customerRepository.GetIdByAccountIdAsync(@event.AccountId, ct);

        if (customerId == null)
        {
            return Result<Guid>.Failure(
                $"Customer with account id: {@event.AccountId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        return Result<Guid>.Success(customerId.Value);
    }
}