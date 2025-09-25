using DotNetEnv;

if (!File.Exists(".env")) throw new Exception(".env file not found");

var loadOptions = new LoadOptions(onlyExactPath: true);
Env.Load(options: loadOptions);

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddConnectionString("DB");

var blobStorage = builder.AddConnectionString("BLOB");

var toxicApi = builder.AddContainer("toxic-api", "iluhahr/toxic-ai-api:latest")
    .WithHttpEndpoint(targetPort: 8000);

builder.AddProject<Projects.Wolfix_API>("api")
    .WithReference(db)
    .WithReference(blobStorage)
    .WithEnvironment("TOXIC_API_BASE_URL", toxicApi.GetEndpoint("http"))
    .WaitFor(toxicApi);

builder.Build().Run();
