using DotNetEnv;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(onlyExactPath: true);
    Env.Load(options: loadOptions);
}

// builder.Host.UseDefaultHostProvider(config =>
// {
//     config.ValidateOnBuild = true;
// });

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddConnectionString("DB");

var blobStorage = builder.AddConnectionString("BLOB");

builder.AddProject<Projects.Wolfix_API>("api")
    .WithReference(db)
    .WithReference(blobStorage);

builder.Build().Run();
