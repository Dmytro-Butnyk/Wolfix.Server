using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Wolfix.Application.Extensions;
using Wolfix.Domain.Catalog.Interfaces;
using Wolfix.Domain.Shared.Interfaces;
using Wolfix.Infrastructure;
using Wolfix.Infrastructure.Catalog.Repositories;
using Wolfix.Infrastructure.Extensions;
using Wolfix.Infrastructure.Shared.Repositories;

namespace Wolfix.API.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();

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
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddRepositories();

        return builder;
    }
    
    public static WebApplicationBuilder AddDbContext(this WebApplicationBuilder builder)
    {
        string connectionString = EnvironmentExtension.GetEnvironmentVariableOrThrow("DB_CONNECTION_STRING");

        builder.Services.AddDbContext<WolfixStoreContext>(options =>
            options.UseNpgsql(connectionString));

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