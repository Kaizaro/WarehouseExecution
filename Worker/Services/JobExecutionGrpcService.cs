using Grpc.Core;
using WarehouseExecution.Application.Jobs.Commands;
using WarehouseExecution.Worker.Grpc;

namespace WarehouseExecution.Worker.Services;

public sealed class JobExecutionGrpcService(IJobCommandService jobCommandService)
    : Grpc.JobExecutionService.JobExecutionServiceBase
{
    public override async Task<ExecuteJobResponse> ExecuteJob(ExecuteJobRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.JobId, out var jobId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "job_id must be a valid GUID."));
        }

        try
        {
            var job = await jobCommandService.ExecuteAsync(jobId, context.CancellationToken);
            var step = job.Steps.Single();

            return new ExecuteJobResponse
            {
                JobId = job.Id.ToString(),
                JobNumber = job.JobNumber,
                Status = job.Status.ToString(),
                StepId = step.Id.ToString(),
                StepNumber = step.StepNumber
            };
        }
        catch (InvalidOperationException exception)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));
        }
    }
}
