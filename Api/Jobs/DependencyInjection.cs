using WarehouseExecution.Api.Jobs.Services;
using WarehouseExecution.Worker.Grpc;

namespace WarehouseExecution.Api.Jobs;

public static class DependencyInjection
{
    public static IServiceCollection AddJobExecutionClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var workerUrl = configuration["Grpc:WorkerUrl"]
                        ?? throw new InvalidOperationException("Grpc:WorkerUrl configuration is missing.");

        services.AddGrpcClient<JobExecutionService.JobExecutionServiceClient>(options =>
        {
            options.Address = new Uri(workerUrl);
        });

        services.AddScoped<IJobExecutionGateway, GrpcJobExecutionGateway>();

        return services;
    }
}
