using Customer.Domain.Interfaces;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using CustomerAggregate = Customer.Domain.CustomerAggregate.Customer;

namespace Customer.Application.EventHandlers;

public sealed class CustomerAccountCreatedViaGoogleEventHandler(ICustomerRepository customerRepository)
    : IIntegrationEventHandler<CustomerAccountCreatedViaGoogle, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CustomerAccountCreatedViaGoogle @event, CancellationToken ct)
    {
        Result<CustomerAggregate> createCustomerResult =
            CustomerAggregate.CreateViaGoogle(@event.AccountId, @event.LastName, @event.FirstName, @event.PhotoUrl);
        
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