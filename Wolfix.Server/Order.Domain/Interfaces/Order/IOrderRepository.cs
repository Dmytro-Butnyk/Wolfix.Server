using Order.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Order.Domain.Interfaces.Order;

public interface IOrderRepository : IBaseRepository<OrderAggregate.Order>
{
    Task<IReadOnlyCollection<CustomerOrderProjection>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct);
    
    Task<OrderDetailsProjection?> GetOrderDetailsAsync(Guid orderId, CancellationToken ct);
    
    Task<IReadOnlyCollection<SellerOrderItemProjection>> GetSellerOrdersAsync(Guid sellerId, CancellationToken ct);
    
    Task<OrderAggregate.Order?> GetCustomerOrderAsync(Guid orderId, Guid customerId, CancellationToken ct);
}