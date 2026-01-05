using DotNetEnv;
using Wolfix.AppHost.Extensions;

LoadOptions options = new(onlyExactPath: true);
var envKeyValues = EnvExtensions.LoadOrThrow(options);

var builder = DistributedApplication.CreateBuilder(args);

var toxicApi = builder.AddContainer("toxic-api", "iluhahr/toxic-ai-api:latest")
    .WithHttpEndpoint(targetPort: 8000);

var mongoDb = builder.AddContainer("mongodb-local", "mongo", "latest")
    .WithEndpoint(targetPort: 27017, port: 27017, name: "mongodb", scheme: "tcp");

builder.AddProject<Projects.Wolfix_API>("api")
    .WithEnvironment("TOXIC_API_BASE_URL", toxicApi.GetEndpoint("http"))
    .WithCustomEnvironmentVariables(envKeyValues)
    .WaitFor(toxicApi)
    .WaitFor(mongoDb);

builder.Build().Run();
