using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseExecution.Infrastructure.Jobs.Execution;
using WarehouseExecution.Infrastructure.Jobs;
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

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IJobNumberGenerator, DbJobNumberGenerator>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IJobExecutionService, JobExecutionService>();

        return services;
    }
}
