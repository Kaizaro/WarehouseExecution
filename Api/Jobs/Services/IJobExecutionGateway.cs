using WarehouseExecution.Worker.Grpc;

namespace WarehouseExecution.Api.Jobs.Services;

public interface IJobExecutionGateway
{
    Task<ExecuteJobResponse> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<CancelJobResponse> CancelAsync(Guid jobId, CancellationToken cancellationToken = default);
}
