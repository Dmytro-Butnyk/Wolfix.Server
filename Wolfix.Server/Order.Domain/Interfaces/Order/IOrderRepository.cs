using Shared.Domain.Interfaces;

namespace Order.Domain.Interfaces.Order;

public interface IOrderRepository : IBaseRepository<OrderAggregate.Order>
{
    
}