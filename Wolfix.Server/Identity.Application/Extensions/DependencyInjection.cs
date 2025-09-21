using Admin.IntegrationEvents;
using Customer.IntegrationEvents;
using Identity.Application.EventHandlers;
using Identity.Application.Interfaces.Services;
using Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Seller.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    public static IServiceCollection AddIdentityEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CustomerWantsToBeSeller>, CustomerWantsToBeSellerEventHandler>();
        services.AddScoped<IIntegrationEventHandler<SellerApplicationApproved>, SellerApplicationApprovedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CreateAdmin, Guid>, CreateAdminEventHandler>();

        return services;
    }
}