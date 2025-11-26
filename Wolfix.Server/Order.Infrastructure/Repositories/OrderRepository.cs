using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces.Order;
using Order.Domain.OrderAggregate.Entities;
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
            .Include("_orderItems")
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => new CustomerOrderProjection(
                order.Id,
                order.Number,
                EF.Property<List<OrderItem>>(order, "_orderItems")
                    .Select(oi => oi.Title)
                    .ToList(),
                order.DeliveryStatus,
                order.PaymentOption,
                order.PaymentStatus,
                order.DeliveryInfo,
                order.DeliveryMethodName,
                order.Price,
                order.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<OrderDetailsProjection?> GetOrderDetailsAsync(Guid orderId, CancellationToken ct)
    {
        return await _orders
            .AsNoTracking()
            .Where(order => order.Id == orderId)
            .Include("_orderItems")
            .Select(order => new OrderDetailsProjection(
                order.Id,
                order.Number,
                order.RecipientInfo.FullName.FirstName,
                order.RecipientInfo.FullName.LastName,
                order.RecipientInfo.FullName.MiddleName,
                order.RecipientInfo.PhoneNumber.Value,
                order.DeliveryStatus,
                order.PaymentOption,
                order.PaymentStatus,
                order.DeliveryInfo.Number,
                order.DeliveryInfo.City,
                order.DeliveryInfo.Street,
                order.DeliveryInfo.HouseNumber,
                order.DeliveryMethodName,
                order.Price,
                EF.Property<List<OrderItem>>(order, "_orderItems").Select(oi =>
                    new OrderItemDetailsProjection(oi.Id, oi.PhotoUrl, oi.Title, oi.Quantity, oi.Price,
                        oi.ProductId)).ToList()))
            .FirstOrDefaultAsync(ct);
    }
}