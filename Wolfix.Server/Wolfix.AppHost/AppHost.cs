using DotNetEnv;
using Wolfix.AppHost.Extensions;

LoadOptions options = new(onlyExactPath: true);
var envKeyValues = EnvExtensions.LoadOrThrow(options);

var builder = DistributedApplication.CreateBuilder(args);

var toxicApi = builder.AddContainer("toxic-api", "iluhahr/toxic-ai-api:latest")
    .WithHttpEndpoint(targetPort: 8000);

builder.AddProject<Projects.Wolfix_API>("api")
    .WithEnvironment("TOXIC_API_BASE_URL", toxicApi.GetEndpoint("http"))
    .WithCustomEnvironmentVariables(envKeyValues)
    .WaitFor(toxicApi);

builder.Build().Run();
