using DotNetEnv;
using Wolfix.AppHost;
using Wolfix.AppHost.Extensions;

LoadOptions options = new(onlyExactPath: true);
EnvExtensions.LoadOrThrow(options);

var builder = DistributedApplication.CreateBuilder(args);

var toxicApi = builder.AddContainer("toxic-api", "iluhahr/toxic-ai-api:latest")
    .WithHttpEndpoint(targetPort: 8000);

builder.AddProject<Projects.Wolfix_API>("api")
    .WithEnvironment("TOXIC_API_BASE_URL", toxicApi.GetEndpoint("http"))
    .WithEnvironment("DB", builder.Configuration.GetOrThrow("DB"))
    .WithEnvironment("BLOB", builder.Configuration.GetOrThrow("BLOB"))
    .WithEnvironment("TOKEN_ISSUER", builder.Configuration.GetOrThrow("TOKEN_ISSUER"))
    .WithEnvironment("TOKEN_AUDIENCE", builder.Configuration.GetOrThrow("TOKEN_AUDIENCE"))
    .WithEnvironment("TOKEN_KEY", builder.Configuration.GetOrThrow("TOKEN_KEY"))
    .WithEnvironment("TOKEN_LIFETIME", builder.Configuration.GetOrThrow("TOKEN_LIFETIME"))
    .WithEnvironment("STRIPE_PUBLISHABLE_KEY", builder.Configuration.GetOrThrow("STRIPE_PUBLISHABLE_KEY"))
    .WithEnvironment("STRIPE_SECRET_KEY", builder.Configuration.GetOrThrow("STRIPE_SECRET_KEY"))
    .WithEnvironment("STRIPE_WEBHOOK_KEY", builder.Configuration.GetOrThrow("STRIPE_WEBHOOK_KEY"))
    .WaitFor(toxicApi);

builder.Build().Run();
