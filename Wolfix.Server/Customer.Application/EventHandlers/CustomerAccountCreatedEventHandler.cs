using Customer.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.EventHandlers;

public sealed class CustomerAccountCreatedEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerAccountCreated>
{
    public async Task<VoidResult> HandleAsync(CustomerAccountCreated @event, CancellationToken ct)
    {
        Result<Customer.Domain.CustomerAggregate.Customer> createCustomerResult =
            Domain.CustomerAggregate.Customer.Create(@event.AccountId);
        
        if (!createCustomerResult.IsSuccess)
        {
            return VoidResult.Failure(createCustomerResult.ErrorMessage!, createCustomerResult.StatusCode);
        }

        await customerRepository.AddAsync(createCustomerResult.Value!, ct);
        
        return VoidResult.Success();
    }
}