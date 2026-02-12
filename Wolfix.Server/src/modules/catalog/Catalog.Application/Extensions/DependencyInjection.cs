using Catalog.Application.EventHandlers;
using Catalog.Application.Services;
using Catalog.Domain.Services;
using Customer.IntegrationEvents;
using Media.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Order.IntegrationEvents;
using Seller.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CategoryService>();
        services.AddScoped<ProductService>();
        services.AddScoped<ProductDomainService>();
        services.AddScoped<CategoryDomainService>();

        return services;
    }

    public static IServiceCollection AddCatalogEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CheckProductExistsForAddingToFavorite>, CheckProductExistsForAddingToFavoriteEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CheckProductExistsForAddingToCart>, CheckProductExistsForAddingToCartEventHandler>();
        services.AddScoped<IIntegrationEventHandler<BlobResourceForProductAdded>, BlobResourceForProductAddedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CustomerWantsToPlaceOrderItems>, CustomerWantsToPlaceOrderItemsEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CheckCategoryExist>, CheckCategoryExistEventHandler>();
        
        return services;
    }
}