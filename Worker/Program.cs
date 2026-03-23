using WarehouseExecution.Infrastructure;
using WarehouseExecution.Worker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.Configure<ExecutionOptions>(builder.Configuration.GetSection("Execution"));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<ExecutionBackgroundService>();
builder.Services.AddSingleton<IExecutionScheduler>(serviceProvider =>
    serviceProvider.GetRequiredService<ExecutionBackgroundService>());
builder.Services.AddHostedService(serviceProvider =>
    serviceProvider.GetRequiredService<ExecutionBackgroundService>());

var app = builder.Build();

app.MapGrpcService<JobExecutionGrpcService>();
app.MapGet("/", () => "Use a gRPC client to communicate with this service.");

await app.RunAsync();
