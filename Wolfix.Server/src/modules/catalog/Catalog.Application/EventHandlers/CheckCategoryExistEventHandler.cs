using System.Net;
using Catalog.Domain.CategoryAggregate;
using Catalog.Domain.Interfaces;
using Seller.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.EventHandlers;

public sealed class CheckCategoryExistEventHandler(ICategoryRepository categoryRepository)
    : IIntegrationEventHandler<CheckCategoryExist>
{
    public async Task<VoidResult> HandleAsync(CheckCategoryExist @event, CancellationToken ct)
    {
        Category? category = await categoryRepository.GetByIdAsNoTrackingAsync(@event.CategoryId, ct, "Parent");

        if (category is null)
        {
            return VoidResult.Failure(
                $"Category with id: {@event.CategoryId} not found",
                HttpStatusCode.NotFound
            );
        }
        
        if (!category.IsChild)
        {
            return VoidResult.Failure($"Category with id: {@event.CategoryId} is not child category");
        }

        if (category.Name != @event.Name)
        {
            return VoidResult.Failure($"Category with id: {@event.CategoryId} has different name");
        }
        
        return VoidResult.Success();
    }
}