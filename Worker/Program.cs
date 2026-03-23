using WarehouseExecution.Infrastructure;
using WarehouseExecution.Worker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<JobExecutionGrpcService>();
app.MapGet("/", () => "Use a gRPC client to communicate with this service.");

await app.RunAsync();
