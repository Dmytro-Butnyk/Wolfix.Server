using DotNetEnv;
using Wolfix.API.Extensions;
using Wolfix.API.Middlewares;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(onlyExactPath: true);
    Env.Load(options: loadOptions);
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddAppCache()
    .AddDbContext()
    .AddRepositories()
    .AddApplicationServices()
    .AddResponseCompression();

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

app.Run();

//todo: постараться убрать регионы все
