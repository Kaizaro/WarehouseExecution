using Grpc.Core;
using WarehouseExecution.Application.Jobs.Commands;
using WarehouseExecution.Worker.Grpc;

namespace WarehouseExecution.Worker.Services;

public sealed class JobExecutionGrpcService(
    IJobCommandService jobCommandService,
    IExecutionScheduler executionScheduler)
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
            executionScheduler.Schedule(job.Id);
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

    public override async Task<CancelJobResponse> CancelJob(CancelJobRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.JobId, out var jobId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "job_id must be a valid GUID."));
        }

        try
        {
            if (!executionScheduler.TryCancel(jobId))
            {
                var cancelledJob = await jobCommandService.CancelAsync(jobId, context.CancellationToken);
                return new CancelJobResponse
                {
                    JobId = cancelledJob.Id.ToString(),
                    Status = cancelledJob.Status.ToString()
                };
            }

            return new CancelJobResponse
            {
                JobId = jobId.ToString(),
                Status = "CancellationRequested"
            };
        }
        catch (InvalidOperationException exception)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, exception.Message));
        }
    }
}
