using Grpc.Core;
using WarehouseExecution.Worker.Grpc;

namespace WarehouseExecution.Api.Jobs.Services;

public sealed class GrpcJobExecutionGateway(JobExecutionService.JobExecutionServiceClient client) : IJobExecutionGateway
{
    public Task<ExecuteJobResponse> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return client.ExecuteJobAsync(
            new ExecuteJobRequest { JobId = jobId.ToString() },
            cancellationToken: cancellationToken).ResponseAsync;
    }

    public Task<CancelJobResponse> CancelAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return client.CancelJobAsync(
            new CancelJobRequest { JobId = jobId.ToString() },
            cancellationToken: cancellationToken).ResponseAsync;
    }
}
