using Microsoft.EntityFrameworkCore;
using Serilog;
using WarehouseExecution.Api.Builder;
using WarehouseExecution.Infrastructure;
using WarehouseExecution.Infrastructure.Logging;
using WarehouseExecution.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Serilog config
Log.Logger = SerilogConfigurator.CreateLogger(builder.Configuration, "WarehouseExecution");
builder.Host.UseSerilog();

// Controllers
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

// Swagger
builder.Services.BuildSwagger();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

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
