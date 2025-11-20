using Catalog.Application.EventHandlers;
using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Domain.Interfaces.DomainServices;
using Catalog.Domain.Services;
using Customer.IntegrationEvents;
using Media.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Order.IntegrationEvents;
using Seller.IntegrationEvents;
using Shared.Application.Caching;
using Shared.Application.Interfaces;
using Shared.IntegrationEvents.Interfaces;

namespace Catalog.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductDomainService, ProductDomainService>();
        services.AddScoped<ICategoryDomainService, CategoryDomainService>();

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