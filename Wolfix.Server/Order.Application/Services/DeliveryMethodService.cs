using System.Net;
using Order.Application.Dto.DeliveryMethod;
using Order.Application.Interfaces;
using Order.Application.Mapping;
using Order.Domain.Interfaces.DeliveryMethod;
using Order.Domain.Projections;
using Shared.Domain.Models;

namespace Order.Application.Services;

internal sealed class DeliveryMethodService(IDeliveryMethodRepository deliveryMethodRepository) : IDeliveryMethodService
{
    public async Task<Result<IReadOnlyCollection<DeliveryMethodDto>>> GetDeliveryMethodsAsync(CancellationToken ct)
    {
        IReadOnlyCollection<DeliveryMethodProjection> deliveryMethods = await deliveryMethodRepository.GetDeliveryMethodsAsync(ct);

        if (deliveryMethods.Count == 0)
        {
            return Result<IReadOnlyCollection<DeliveryMethodDto>>.Failure(
                "Delivery methods not found",
                HttpStatusCode.NotFound
            );
        }
        
        IReadOnlyCollection<DeliveryMethodDto> dto = deliveryMethods
            .Select(dm => dm.ToDto())
            .ToList();

        return Result<IReadOnlyCollection<DeliveryMethodDto>>.Success(dto);
    }
}