using Wolfix.API.ExceptionHandlers;
using Wolfix.API.Extensions;
using Wolfix.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

//todo: перенести EventBus в Shared.Application???
//todo: написать юнит тесты в каждом модуле и сделать CI/CD
//todo: написать отдельный проект для заполнения базы

builder
    .AddAppCache()
    .AddEventBus()
    .AddResponseCompression()
    .AddAllModules()
    .AddCors();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseResponseCompression();

app.MapAllEndpoints();

app.UseCors("AllowNextClient");

await app.EnsureAllRolesExist();

app.Run();