using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Domain.Enums;

namespace WarehouseExecution.Application.Jobs.Commands;

public sealed class JobCommandService(
    IJobRepository jobRepository,
    IJobNumberGenerator jobNumberGenerator) : IJobCommandService
{
    public async Task<Job> CreateAsync(
        string fromLocation,
        string toLocation,
        string? productCode,
        string? productName,
        CancellationToken cancellationToken = default)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = await jobNumberGenerator.NextAsync(cancellationToken),
            Status = JobStatus.Created,
            FromLocation = fromLocation,
            ToLocation = toLocation,
            ProductCode = productCode,
            ProductName = productName
        };

        await jobRepository.AddAsync(job, cancellationToken);

        return job;
    }

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

    public async Task<Job> StartExecutionAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, cancellationToken)
                  ?? throw new InvalidOperationException($"Job '{jobId}' was not found.");

        if (job.Status != JobStatus.Planned)
        {
            throw new InvalidOperationException(
                $"Job '{jobId}' cannot start execution from status '{job.Status}'.");
        }

        var step = GetSingleStep(job, jobId);
        step.Status = JobStepStatus.InProgress;
        job.Status = JobStatus.InProgress;

        await jobRepository.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<Job> CompleteAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, cancellationToken)
                  ?? throw new InvalidOperationException($"Job '{jobId}' was not found.");

        if (job.Status != JobStatus.InProgress)
        {
            throw new InvalidOperationException(
                $"Job '{jobId}' cannot be completed from status '{job.Status}'.");
        }

        var step = GetSingleStep(job, jobId);

        if (step.Status != JobStepStatus.InProgress)
        {
            throw new InvalidOperationException(
                $"Job '{jobId}' step cannot be completed from status '{step.Status}'.");
        }

        step.Status = JobStepStatus.Completed;
        job.Status = JobStatus.Completed;

        await jobRepository.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<Job> CancelAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetByIdAsync(jobId, cancellationToken)
                  ?? throw new InvalidOperationException($"Job '{jobId}' was not found.");

        if (job.Status is JobStatus.Completed or JobStatus.Failed or JobStatus.Cancelled)
        {
            throw new InvalidOperationException(
                $"Job '{jobId}' cannot be cancelled from status '{job.Status}'.");
        }

        var step = GetSingleStep(job, jobId);
        step.Status = JobStepStatus.Cancelled;
        job.Status = JobStatus.Cancelled;

        await jobRepository.SaveChangesAsync(cancellationToken);

        return job;
    }

    private static JobStep GetSingleStep(Job job, Guid jobId)
    {
        if (job.Steps.Count != 1)
        {
            throw new InvalidOperationException(
                $"Job '{jobId}' must contain exactly one step in the current prototype.");
        }

        return job.Steps.Single();
    }
}
