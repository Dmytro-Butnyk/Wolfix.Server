using System.Text;
using Admin.IntegrationEvents;
using Identity.Application.EventHandlers;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Seller.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using Support.IntegrationEvents;

namespace Identity.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<JwtService>();

        return services;
    }

    public static IServiceCollection AddIdentityEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<CustomerWantsToBeSeller>, CustomerWantsToBeSellerEventHandler>();
        services.AddScoped<IIntegrationEventHandler<SellerApplicationApproved>, SellerApplicationApprovedEventHandler>();
        services.AddScoped<IIntegrationEventHandler<DeleteSellerAccount>, DeleteSellerAccountEventHandler>();
        services.AddScoped<IIntegrationEventHandler<DeleteSupportAccount>, DeleteSupportAccountEventHandler>();
        services.AddScoped<IIntegrationEventHandler<DeleteAdminAccount>, DeleteAdminAccountEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CreateAdmin, Guid>, CreateAdminEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CreateSupport, Guid>, CreateSupportEventHandler>();

        return services;
    }
    
    public static IServiceCollection AddJwtBearer(this IServiceCollection services, string tokenKey)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                //todo: добавить настройки валидации jwt token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                };
            });

        services.AddAuthorizationBuilder()
            .AddPoliciesByRoles();
        
        return services;
    }

    private static void AddPoliciesByRoles(this AuthorizationBuilder builder)
    {
        foreach (string role in Roles.All)
        {
            builder.AddPolicy(role, policy => policy.RequireRole(role));
        }
    }
}