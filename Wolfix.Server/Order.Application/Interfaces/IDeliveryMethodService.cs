using Order.Application.Dto.DeliveryMethod;
using Shared.Domain.Models;

namespace Order.Application.Interfaces;

public interface IDeliveryMethodService
{
    Task<Result<IReadOnlyCollection<DeliveryMethodDto>>> GetDeliveryMethodsAsync(CancellationToken ct);
}