using System.Text;
using Admin.IntegrationEvents;
using Customer.IntegrationEvents;
using Identity.Application.EventHandlers;
using Identity.Application.Interfaces.Services;
using Identity.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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