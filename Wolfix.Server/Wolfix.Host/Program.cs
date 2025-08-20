using Catalog.Endpoints.Endpoints;
using Catalog.Endpoints.Extensions;
using DotNetEnv;
using Identity.Endpoints.Extensions;
using Wolfix.Host.ExceptionHandlers;
using Wolfix.Host.Extensions;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(onlyExactPath: true);
    Env.Load(options: loadOptions);
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

//todo: перенести EventBus в Shared.Application???
//todo: написать юнит тесты в каждом модуле и сделать CI/CD

builder
    .AddAppCache()
    .AddEventBus()
    .AddResponseCompression()
    .AddAllModules();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseResponseCompression();

app.MapAllEndpoints();

app.Run();