using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces.Order;
using Order.Domain.Projections;
using Shared.Infrastructure.Repositories;

namespace Order.Infrastructure.Repositories;

public sealed class OrderRepository(OrderContext context)
    : BaseRepository<OrderContext, Domain.OrderAggregate.Order>(context), IOrderRepository
{
    private readonly DbSet<Domain.OrderAggregate.Order> _orders = context.Orders;
    
    public async Task<IReadOnlyCollection<CustomerOrderProjection>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct)
    {
        return await _orders
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => new CustomerOrderProjection(
                order.Id,
                order.Number,
                order.PaymentOption,
                order.PaymentStatus,
                order.DeliveryInfo,
                order.DeliveryMethodName,
                order.Price,
                order.CreatedAt))
            .ToListAsync(ct);
    }
}