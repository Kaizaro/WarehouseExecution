using Serilog;
using WarehouseExecution.Api.Builder;
using WarehouseExecution.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Serilog config
Log.Logger = SerilogConfigurator.CreateLogger(builder.Configuration, "WarehouseExecution");
builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.BuildSwagger();

var app = builder.Build();

app.UseSerilogRequestLogging();

/* Basically should be only in dev,
 but for demo reasons it's ok to show */
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(Swagger.SwaggerJsonUrl, Swagger.SwaggerName);
    options.RoutePrefix = Swagger.SwaggerPrefix;
});

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
