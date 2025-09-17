using Catalog.Endpoints.Extensions;
using Customer.Endpoints.Extensions;
using Identity.Endpoints.Extensions;
using Media.Api;
using Microsoft.AspNetCore.ResponseCompression;
using Order.Endpoints.Extensions;
using Seller.Endpoints.Extensions;
using Shared.Application.Extensions;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;

namespace Wolfix.API.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddAllModules(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString("DB")!;

        builder
            .AddCatalogModule(connectionString)
            .AddIdentityModule(connectionString)
            .AddCustomerModule(connectionString)
            .AddMediaModule(connectionString)
            .AddSellerModule(connectionString)
            .AddOrderModule(connectionString);
        
        return builder;
    }
    
    private static WebApplicationBuilder AddMediaModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddMediaModule(connectionString, builder.Configuration);
        
        return builder;
    }

    private static WebApplicationBuilder AddCatalogModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddCatalogModule(connectionString);

        return builder;
    }

    private static WebApplicationBuilder AddIdentityModule(this WebApplicationBuilder builder, string connectionString)
    {
        string tokenIssuer = builder.Configuration.GetOrThrow("TOKEN_ISSUER");
        string tokenAudience = builder.Configuration.GetOrThrow("TOKEN_AUDIENCE");
        string tokenKey = builder.Configuration.GetOrThrow("TOKEN_KEY");
        string tokenLifetime = builder.Configuration.GetOrThrow("TOKEN_LIFETIME");
        
        builder.Services.AddIdentityModule(connectionString, tokenIssuer, tokenAudience, tokenKey, tokenLifetime);
        
        return builder;
    }

    private static WebApplicationBuilder AddCustomerModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddCustomerModule(connectionString);
        
        return builder;
    }

    private static WebApplicationBuilder AddSellerModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddSellerModule(connectionString);

        return builder;
    }

    private static WebApplicationBuilder AddOrderModule(this WebApplicationBuilder builder, string connectionString)
    {
        string publishableKey = builder.Configuration.GetOrThrow("STRIPE_PUBLISHABLE_KEY");
        string secretKey = builder.Configuration.GetOrThrow("STRIPE_SECRET_KEY");
        string webhookKey = builder.Configuration.GetOrThrow("STRIPE_WEBHOOK_KEY");
        
        builder.Services.AddOrderModule(connectionString, publishableKey, secretKey, webhookKey);
        
        return builder;
    }

    public static WebApplicationBuilder AddEventBus(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEventBus, EventBus>();
        
        return builder;
    }
    
    //
    // public static WebApplicationBuilder AddIntegrationServices(this WebApplicationBuilder builder)
    // {
    //     
    // }
    //
    // public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    // {
    //     
    // }
    //
    public static WebApplicationBuilder AddAppCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();

        builder.Services.AddAppCache();
        
        return builder;
    }
    //
    // public static WebApplicationBuilder AddFluentValidation(this WebApplicationBuilder builder)
    // {
    //     
    // }
    //
    // public static WebApplicationBuilder AddSerilogLogger(this WebApplicationBuilder builder)
    // {
    //     
    // }
    //
    public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowNextClient", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return builder;
    }
    //
    // public static WebApplicationBuilder AddJwtBearer(this WebApplicationBuilder builder)
    // {
    //     
    // }
    //
    public static WebApplicationBuilder AddResponseCompression(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        return builder;
    }
    //
    // public static WebApplicationBuilder AddRateLimiter(this WebApplicationBuilder builder)
    // {
    //     
    // }
    //
    // private static void AddRateLimiterPolicy(RateLimiterOptions options, string policyName, int limit,
    //     TimeSpan expiration)
    // {
    //     
    // }
    //
    // public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
    // {
    //     
    // }
}