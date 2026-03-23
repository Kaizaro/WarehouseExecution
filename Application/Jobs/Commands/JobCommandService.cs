using Domain.Enums;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Domain.Entities;

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
}
