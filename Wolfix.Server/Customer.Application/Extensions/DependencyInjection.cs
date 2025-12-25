using Catalog.IntegrationEvents;
using Customer.Application.EventHandlers;
using Customer.Application.Services;
using Identity.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;
using Order.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;
using Support.IntegrationEvents.Dto;

namespace Customer.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CustomerService>();
        
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
        services.AddScoped<IIntegrationEventHandler<CustomerOrderCreated>, CustomerOrderCreatedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<FetchCustomerInformationForCreatingSupportRequest, CustomerInformationForSupportRequestDto>, FetchCustomerInformationForCreatingSupportRequestEventHandler>();

        return services;
    }
}