using Order.Application.Dto.Order.Requests;
using Shared.Domain.Models;

namespace Order.Application.Interfaces;

public interface IOrderService
{
    Task<Result<string>> PlaceOrderAsync(PlaceOrderDto request, CancellationToken ct);
}