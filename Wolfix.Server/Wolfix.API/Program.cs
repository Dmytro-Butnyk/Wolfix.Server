using DotNetEnv;
using Wolfix.API.Extensions;
using Wolfix.API.Middlewares;
using Wolfix.Infrastructure.Extensions;

if (File.Exists(".env"))
{
    var loadOptions = new LoadOptions(onlyExactPath: true);
    Env.Load(options: loadOptions);
}

var builder = WebApplication.CreateBuilder(args);

builder
    .AddDbContext()
    .AddResponseCompression();

builder.Services
    .AddRepositories();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseStaticFiles();

app.UseMiddleware<ExceptionMiddleware>();

app.UseResponseCompression();

app.Run();
