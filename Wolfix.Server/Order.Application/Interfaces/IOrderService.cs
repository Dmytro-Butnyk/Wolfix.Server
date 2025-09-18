using Order.Application.Dto.Order.Requests;
using Shared.Domain.Models;

namespace Order.Application.Interfaces;

public interface IOrderService
{
    Task<Result<string>> PlaceOrderWithPaymentAsync(PlaceOrderDto request, CancellationToken ct);
    
    Task<VoidResult> PlaceOrderAsync(PlaceOrderDto request, CancellationToken ct);
    
    Task<VoidResult> MarkOrderPaid(Guid orderId, CancellationToken ct);
}