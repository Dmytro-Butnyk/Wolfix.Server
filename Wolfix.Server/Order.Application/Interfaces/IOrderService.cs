using Order.Application.Dto.Order.Requests;
using Order.Application.Dto.Order.Responses;
using Shared.Domain.Models;

namespace Order.Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderPlacedWithPaymentDto>> PlaceOrderWithPaymentAsync(PlaceOrderDto request, CancellationToken ct);
    
    Task<VoidResult> PlaceOrderAsync(PlaceOrderDto request, CancellationToken ct);
    
    Task<VoidResult> MarkOrderPaidAsync(Guid orderId, CancellationToken ct);
    
    Task<Result<IReadOnlyCollection<CustomerOrderDto>>> GetCustomerOrdersAsync(Guid customerId, CancellationToken ct);
    
    Task<Result<OrderDetailsDto>> GetOrderDetailsAsync(Guid orderId, CancellationToken ct);
}