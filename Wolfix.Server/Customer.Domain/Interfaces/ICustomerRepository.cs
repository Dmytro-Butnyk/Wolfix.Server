using Shared.Domain.Interfaces;

namespace Customer.Domain.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer.Domain.CustomerAggregate.Customer>
{
    
}