using Domain.Enums;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Jobs.Repositories;

namespace WarehouseExecution.Infrastructure.Jobs.Execution;

public sealed class JobExecutionService(IJobRepository jobRepository) : IJobExecutionService
{
    public async Task<Job> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, cancellationToken)
                  ?? throw new InvalidOperationException($"Job '{jobId}' was not found.");

        if (job.Status != JobStatus.Created)
        {
            throw new InvalidOperationException(
                $"Job '{jobId}' cannot be executed from status '{job.Status}'.");
        }

        if (job.Steps.Count > 0)
        {
            throw new InvalidOperationException($"Job '{jobId}' already has planned steps.");
        }

        var step = new JobStep
        {
            Id = Guid.NewGuid(),
            JobId = job.Id,
            StepNumber = 1,
            Status = JobStepStatus.Pending,
            FromLocation = job.FromLocation,
            ToLocation = job.ToLocation
        };

        job.Steps.Add(step);
        job.Status = JobStatus.Planned;

        await jobRepository.SaveChangesAsync(cancellationToken);

        return job;
    }
}
