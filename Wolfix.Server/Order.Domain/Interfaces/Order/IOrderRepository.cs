using Order.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Order.Domain.Interfaces.Order;

public interface IOrderRepository : IBaseRepository<OrderAggregate.Order>
{
    Task<IReadOnlyCollection<CustomerOrderProjection>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct);
}