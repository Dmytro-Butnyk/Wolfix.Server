using Catalog.Endpoints.Extensions;
using Customer.Endpoints.Extensions;
using Identity.Endpoints.Extensions;
using Media.Api;
using Microsoft.AspNetCore.ResponseCompression;
using Shared.Application.Extensions;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Inerfaces;

namespace Wolfix.Host.Extensions;

public static class WebApplicationBuilderExtension
{
    
    public static WebApplicationBuilder AddAllModules(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtension.GetEnvironmentVariableOrThrow("DB_CONNECTION_STRING");

        builder
            .AddCatalogModule(connectionString)
            .AddIdentityModule(connectionString)
            .AddCustomerModule(connectionString)
            .AddMediaModule(connectionString);
        
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
        string tokenIssuer = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_ISSUER");
        string tokenAudience = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_AUDIENCE");
        string tokenKey = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_KEY");
        string tokenLifetime = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_LIFETIME");
        
        builder.Services.AddIdentityModule(connectionString, tokenIssuer, tokenAudience, tokenKey, tokenLifetime);
        
        return builder;
    }

    private static WebApplicationBuilder AddCustomerModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddCustomerModule(connectionString);
        
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
    // public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
    // {
    //     
    // }
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