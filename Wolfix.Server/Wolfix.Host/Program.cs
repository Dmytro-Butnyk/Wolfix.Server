using Catalog.Endpoints.Endpoints;
using Catalog.Endpoints.Extensions;
using DotNetEnv;
using Identity.Endpoints.Extensions;
using Wolfix.Host.Extensions;
using Wolfix.Host.Middlewares;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(onlyExactPath: true);
    Env.Load(options: loadOptions);
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

//todo: проверить каждый резалт фейлур!!!
//todo: перенести EventBus в Shared.Application???
//todo: добавить свойство PhotoUrl в Customer

builder
    .AddAppCache()
    .AddEventBus()
    .AddResponseCompression()
    .AddAllModules();

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

app.UseMiddleware<ExceptionMiddleware>();

app.UseResponseCompression();

app.MapAllEndpoints();

app.Run();