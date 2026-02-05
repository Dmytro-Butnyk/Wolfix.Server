using Admin.Endpoints.Extensions;
using Catalog.Endpoints.Extensions;
using Customer.Endpoints.Extensions;
using Identity.Endpoints.Extensions;
using Media.Api;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Order.Endpoints.Extensions;
using Seller.Endpoints.Extensions;
using Serilog;
using Shared.Application.Extensions;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using Support.Application.Dto.SupportRequest.Create;
using Support.Endpoints.Extensions;
using Support.Infrastructure.MongoDB.Extensions;

namespace Wolfix.API.Extensions;

public static class WebApplicationBuilderExtension
{
    public static async Task<WebApplicationBuilder> AddAllModules(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetOrThrow("DB");

        builder
            .AddCatalogModule(connectionString)
            .AddIdentityModule(connectionString)
            .AddCustomerModule(connectionString)
            .AddMediaModule(connectionString)
            .AddSellerModule(connectionString)
            .AddOrderModule(connectionString)
            .AddAdminModule(connectionString);

        await builder.AddSupportModule();
        
        return builder;
    }
    
    private static WebApplicationBuilder AddMediaModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddMediaModule(connectionString, builder.Configuration);
        
        return builder;
    }

    private static WebApplicationBuilder AddCatalogModule(this WebApplicationBuilder builder, string connectionString)
    {
        string toxicApiBaseUrl = builder.Configuration.GetOrThrow("TOXIC_API_BASE_URL");
        
        builder.Services.AddCatalogModule(connectionString, toxicApiBaseUrl);

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

    private static async Task<WebApplicationBuilder> AddSupportModule(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetOrThrow("MONGODB_CONNECTION_STRING");
        string databaseName = builder.Configuration.GetOrThrow("MONGODB_DATABASE_NAME");
        
        builder.Services.AddSupportModule(connectionString, databaseName);
        await builder.Services.BuildServiceProvider().AddSupportMongoDbIndexes();
        
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });
        
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

    private static WebApplicationBuilder AddAdminModule(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddAdminModule(connectionString);

        return builder;
    }

    public static WebApplicationBuilder AddEventBus(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<EventBus>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddAppCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();

        builder.Services.AddAppCache();
        
        return builder;
    }
    
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
    
    public static WebApplicationBuilder AddSwaggerJwtBearer(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.UseOneOfForPolymorphism();
            
            options.SelectDiscriminatorNameUsing(baseType =>
                baseType.Name == nameof(CreateSupportRequestDto) ? "kind" : null);
            
            options.SelectSubTypesUsing(baseType =>
            {
                if (baseType == typeof(CreateSupportRequestDto))
                {
                    return typeof(Program).Assembly.GetTypes()
                       .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);
                }

                return [];
            });
            
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description =
                    "Input your JWT token in the 'Authorization' header like this: \"Authorization: Bearer {yourJWT}\""
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }
    
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

    public static WebApplicationBuilder AddLoggingToMongoDb(this WebApplicationBuilder builder)
    {
        string databaseUrl = builder.Configuration.GetOrThrow("MONGODB_LOGGING_DATABASE_URL");
        string collectionName = builder.Configuration.GetOrThrow("MONGODB_LOGGING_COLLECTION_NAME");

        builder.Host.UseSerilog((context, _, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .WriteTo.MongoDBBson(
                databaseUrl,
                collectionName: collectionName,
                cappedMaxSizeMb: 100)
            .Enrich.FromLogContext());
        
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

    public static void ValidateDIOnBuild(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider((context, options) =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });
    }
}