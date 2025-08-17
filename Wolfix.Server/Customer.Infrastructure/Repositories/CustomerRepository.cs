using Customer.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Customer.Infrastructure.Repositories;

public sealed class CustomerRepository(CustomerContext context)
    : BaseRepository<CustomerContext, Customer.Domain.CustomerAggregate.Customer>(context), ICustomerRepository
{
    //todo: доделать
}