using Order.Domain.Interfaces.Order;
using Shared.Infrastructure.Repositories;

namespace Order.Infrastructure.Repositories;

public sealed class OrderRepository(OrderContext context)
    : BaseRepository<OrderContext, Domain.OrderAggregate.Order>(context), IOrderRepository
{
    
}