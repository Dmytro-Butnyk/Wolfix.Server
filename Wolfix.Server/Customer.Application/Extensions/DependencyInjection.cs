using Catalog.IntegrationEvents;
using Customer.Application.EventHandlers;
using Customer.Application.Interfaces;
using Customer.Application.Services;
using Customer.IntegrationEvents;
using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Notification.IntegrationEvents;
using Order.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Customer.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        
        return services;
    }

    public static IServiceCollection AddCustomerEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CustomerAccountCreated, Guid>, CustomerAccountCreatedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<ProductExistsForAddingToFavorite>, ProductExistsForAddingToFavoriteEventHandler>();
        services.AddScoped<IIntegrationEventHandler<ProductExistsForAddingToCart>, ProductExistsForAddingToCartEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CheckCustomerExistsForAddingReview>, CheckCustomerExistsForAddingReviewEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CustomerWantsToPlaceOrder>, CustomerWantsToPlaceOrderEventHandler>();
        services.AddScoped<IIntegrationEventHandler<GetCustomerProfileId, Guid>, GetCustomerProfileIdEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CustomerWantsToGetOrders>, CustomerWantsToGetOrdersEventHandler>();
        services.AddScoped<IIntegrationEventHandler<FetchUserFavorites>, FetchUserFavoritesEventHandler>();

        return services;
    }
}