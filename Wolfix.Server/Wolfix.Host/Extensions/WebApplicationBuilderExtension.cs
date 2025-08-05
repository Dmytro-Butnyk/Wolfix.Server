using Catalog.API.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Extensions;

namespace Wolfix.Host.Extensions;

public static class WebApplicationBuilderExtension
{
    private static string GetDbConnectionString()
    {
        return EnvironmentExtension.GetEnvironmentVariableOrThrow("DB_CONNECTION_STRING");
    }
    
    public static WebApplicationBuilder AddCatalogModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddCatalogModule(GetDbConnectionString());

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