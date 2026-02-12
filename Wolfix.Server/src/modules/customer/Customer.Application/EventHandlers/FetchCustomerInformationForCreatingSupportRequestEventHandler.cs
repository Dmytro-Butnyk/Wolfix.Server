using Customer.Domain.Interfaces;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;
using Support.IntegrationEvents.Dto;

namespace Customer.Application.EventHandlers;

public sealed class FetchCustomerInformationForCreatingSupportRequestEventHandler(
    ICustomerRepository customerRepository)
    : IIntegrationEventHandler<FetchCustomerInformationForCreatingSupportRequest,
        CustomerInformationForSupportRequestDto>
{
    public async Task<Result<CustomerInformationForSupportRequestDto>> HandleAsync(FetchCustomerInformationForCreatingSupportRequest @event,
        CancellationToken ct)
    {
        Domain.CustomerAggregate.Customer? customer = await customerRepository.GetByIdAsync(@event.CustomerId, ct);

        if (customer is null)
        {
            return Result<CustomerInformationForSupportRequestDto>.Failure("Customer not found");
        }

        CustomerInformationForSupportRequestDto customerInformationForSupportRequestDto
            = new CustomerInformationForSupportRequestDto(
                customer.GetFirstName(),
                customer.GetLastName(),
                customer.GetMiddleName(),
                customer.GetPhoneNumber(),
                customer.GetBirthDate()
            );

        return Result<CustomerInformationForSupportRequestDto>.Success(customerInformationForSupportRequestDto);
    }
}