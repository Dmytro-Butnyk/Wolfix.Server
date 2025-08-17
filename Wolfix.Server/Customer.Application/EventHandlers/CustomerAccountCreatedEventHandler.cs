using Customer.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Inerfaces;

namespace Customer.Application.EventHandlers;

public sealed class CustomerAccountCreatedEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerAccountCreated>
{
    //todo: прописать нормально
    public async Task HandleAsync(CustomerAccountCreated @event, CancellationToken ct)
    {
        Result<Customer.Domain.CustomerAggregate.Customer> createCustomerResult =
            Domain.CustomerAggregate.Customer.Create(@event.AccountId);
        
        if (!createCustomerResult.IsSuccess)
        {
            throw new Exception(createCustomerResult.ErrorMessage);
        }

        await customerRepository.AddAsync(createCustomerResult.Value!, ct);
    }
}