using WarehouseExecution.Application.Jobs.Queries;

namespace WarehouseExecution.Api.Jobs.Contracts;

public static class JobMappings
{
    public static JobResponse ToResponse(this JobView job)
    {
        return new JobResponse
        {
            Id = job.Id,
            JobNumber = job.JobNumber,
            Status = job.Status,
            ProductCode = job.ProductCode,
            ProductName = job.ProductName,
            FromLocation = job.FromLocationCode,
            ToLocation = job.ToLocationCode,
            CreatedAtUtc = job.CreatedAtUtc,
            UpdatedAtUtc = job.UpdatedAtUtc,
            Steps = job.Steps.Select(step => new JobStepResponse
            {
                Id = step.Id,
                StepNumber = step.StepNumber,
                Status = step.Status,
                FromLocation = step.FromLocationCode,
                ToLocation = step.ToLocationCode,
                CreatedAtUtc = step.CreatedAtUtc,
                UpdatedAtUtc = step.UpdatedAtUtc
            }).ToList()
        };
    }
}
