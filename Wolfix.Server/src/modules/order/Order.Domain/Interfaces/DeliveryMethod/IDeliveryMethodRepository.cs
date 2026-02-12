using Order.Domain.Projections;
using Shared.Domain.Interfaces;

namespace Order.Domain.Interfaces.DeliveryMethod;

public interface IDeliveryMethodRepository : IBaseRepository<DeliveryAggregate.DeliveryMethod>
{
    Task<IReadOnlyCollection<DeliveryMethodProjection>> GetDeliveryMethodsAsync(CancellationToken ct);
}