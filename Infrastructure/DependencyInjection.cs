using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Application.Jobs.Commands;
using WarehouseExecution.Application.Jobs.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseExecution.Infrastructure.Jobs;
using WarehouseExecution.Infrastructure.Jobs.JobNumberGenerator;
using WarehouseExecution.Infrastructure.Jobs.Repositories;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringName = "WarehouseExecutionDb";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
                               ?? throw new InvalidOperationException(
                                   $"Connection string '{ConnectionStringName}' was not found.");

        //DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        // Add DI services
        services.AddScoped<IJobNumberGenerator, DbJobNumberGenerator>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IJobCommandService, JobCommandService>();
        services.AddScoped<IJobQueryService, JobQueryService>();

        return services;
    }
}
