using Catalog.Endpoints.Extensions;
using Identity.Endpoints.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Shared.Application.Extensions;
using Shared.Infrastructure.Extensions;

namespace Wolfix.Host.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddSharedRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddSharedRepositories();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddCatalogModule(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtension.GetEnvironmentVariableOrThrow("DB_CONNECTION_STRING");
        
        builder.Services.AddCatalogModule(connectionString);

        return builder;
    }

    public static WebApplicationBuilder AddIdentityModule(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtension.GetEnvironmentVariableOrThrow("DB_CONNECTION_STRING");
        string tokenIssuer = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_ISSUER");
        string tokenAudience = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_AUDIENCE");
        string tokenKey = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_KEY");
        string tokenLifetime = EnvironmentExtension.GetEnvironmentVariableOrThrow("TOKEN_LIFETIME");
        
        builder.Services.AddIdentityModule(connectionString, tokenIssuer, tokenAudience, tokenKey, tokenLifetime);
        
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