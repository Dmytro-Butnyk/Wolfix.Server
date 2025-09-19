using Customer.Domain.Interfaces;
using Customer.IntegrationEvents;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using CustomerAggregate = Customer.Domain.CustomerAggregate.Customer;

namespace Customer.Application.EventHandlers;

internal sealed class CustomerAccountCreatedEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerAccountCreated, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CustomerAccountCreated @event, CancellationToken ct)
    {
        Result<CustomerAggregate> createCustomerResult =
            CustomerAggregate.Create(@event.AccountId);
        
        if (createCustomerResult.IsFailure)
        {
            return Result<Guid>.Failure(createCustomerResult);
        }
        
        CustomerAggregate customer = createCustomerResult.Value!;

        await customerRepository.AddAsync(customer, ct);
        
        await customerRepository.SaveChangesAsync(ct);
        
        return Result<Guid>.Success(customer.Id);
    }
}