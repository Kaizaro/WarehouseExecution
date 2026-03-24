using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Application.Common;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Domain.Enums;

namespace WarehouseExecution.Application.Jobs.Commands;

public sealed class JobCommandService(
    IJobRepository jobRepository,
    IJobNumberGenerator jobNumberGenerator,
    ILocationRepository locationRepository) : IJobCommandService
{
    public async Task<Job> CreateAsync(
        string fromLocation,
        string toLocation,
        string? productCode,
        string? productName,
        CancellationToken cancellationToken = default)
    {
        var normalizedFromLocation = NormalizeLocationCode(fromLocation, "Source location");
        var normalizedToLocation = NormalizeLocationCode(toLocation, "Destination location");

        if (string.Equals(normalizedFromLocation, normalizedToLocation, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Source and destination locations must be different.");
        }

        var sourceLocation = await locationRepository.GetByCodeAsync(normalizedFromLocation, cancellationToken)
                             ?? throw new NotFoundException(
                                 $"Source location '{normalizedFromLocation}' was not found.");

        var destinationLocation = await locationRepository.GetByCodeAsync(normalizedToLocation, cancellationToken)
                                  ?? throw new NotFoundException(
                                      $"Destination location '{normalizedToLocation}' was not found.");

        var job = new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = await jobNumberGenerator.NextAsync(cancellationToken),
            Status = JobStatus.Created,
            FromLocationId = sourceLocation.Id,
            ToLocationId = destinationLocation.Id,
            ProductCode = productCode,
            ProductName = productName
        };

        await jobRepository.AddAsync(job, cancellationToken);

        return job;
    }

    public async Task<Job> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetForUpdateAsync(jobId, cancellationToken)
                  ?? throw new NotFoundException($"Job '{jobId}' was not found.");

        if (job.Status != JobStatus.Created)
        {
            throw new ConflictException(
                $"Job '{jobId}' cannot be executed from status '{job.Status}'.");
        }

        if (job.Steps.Count > 0)
        {
            throw new ConflictException($"Job '{jobId}' already has planned steps.");
        }

        var step = new JobStep
        {
            Id = Guid.NewGuid(),
            JobId = job.Id,
            StepNumber = 1,
            Status = JobStepStatus.Pending,
            FromLocationId = job.FromLocationId,
            ToLocationId = job.ToLocationId
        };

        job.Steps.Add(step);
        job.Status = JobStatus.Planned;

        await jobRepository.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<Job> StartExecutionAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetForUpdateAsync(jobId, cancellationToken)
                  ?? throw new NotFoundException($"Job '{jobId}' was not found.");

        if (job.Status != JobStatus.Planned)
        {
            throw new ConflictException(
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
        var job = await jobRepository.GetForUpdateAsync(jobId, cancellationToken)
                  ?? throw new NotFoundException($"Job '{jobId}' was not found.");

        if (job.Status != JobStatus.InProgress)
        {
            throw new ConflictException(
                $"Job '{jobId}' cannot be completed from status '{job.Status}'.");
        }

        var step = GetSingleStep(job, jobId);

        if (step.Status != JobStepStatus.InProgress)
        {
            throw new ConflictException(
                $"Job '{jobId}' step cannot be completed from status '{step.Status}'.");
        }

        step.Status = JobStepStatus.Completed;
        job.Status = JobStatus.Completed;

        await jobRepository.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<Job> CancelAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetForUpdateAsync(jobId, cancellationToken)
                  ?? throw new NotFoundException($"Job '{jobId}' was not found.");

        if (job.Status is JobStatus.Completed or JobStatus.Failed or JobStatus.Cancelled)
        {
            throw new ConflictException(
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
            throw new ConflictException(
                $"Job '{jobId}' must contain exactly one step in the current prototype.");
        }

        return job.Steps.Single();
    }

    private static string NormalizeLocationCode(string locationCode, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(locationCode))
        {
            throw new ValidationException($"{fieldName} is required.");
        }

        return locationCode.Trim();
    }
}
