using Microsoft.EntityFrameworkCore;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Application.Jobs.Queries;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public sealed class JobRepository(AppDbContext dbContext) : IJobRepository
{
    public async Task<IReadOnlyList<JobView>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(job => new JobView
            {
                Id = job.Id,
                JobNumber = job.JobNumber,
                Status = job.Status.ToString(),
                ProductCode = job.ProductCode,
                ProductName = job.ProductName,
                FromLocationCode = dbContext.Locations
                    .Where(location => location.Id == job.FromLocationId)
                    .Select(location => location.Code)
                    .Single(),
                ToLocationCode = dbContext.Locations
                    .Where(location => location.Id == job.ToLocationId)
                    .Select(location => location.Code)
                    .Single(),
                CreatedAtUtc = job.CreatedAtUtc,
                UpdatedAtUtc = job.UpdatedAtUtc,
                Steps = job.Steps
                    .OrderBy(step => step.StepNumber)
                    .Select(step => new JobStepView
                    {
                        Id = step.Id,
                        StepNumber = step.StepNumber,
                        Status = step.Status.ToString(),
                        FromLocationCode = dbContext.Locations
                            .Where(location => location.Id == step.FromLocationId)
                            .Select(location => location.Code)
                            .Single(),
                        ToLocationCode = dbContext.Locations
                            .Where(location => location.Id == step.ToLocationId)
                            .Select(location => location.Code)
                            .Single(),
                        CreatedAtUtc = step.CreatedAtUtc,
                        UpdatedAtUtc = step.UpdatedAtUtc
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public Task<JobView?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Jobs
            .Where(job => job.Id == id)
            .Select(job => new JobView
            {
                Id = job.Id,
                JobNumber = job.JobNumber,
                Status = job.Status.ToString(),
                ProductCode = job.ProductCode,
                ProductName = job.ProductName,
                FromLocationCode = dbContext.Locations
                    .Where(location => location.Id == job.FromLocationId)
                    .Select(location => location.Code)
                    .Single(),
                ToLocationCode = dbContext.Locations
                    .Where(location => location.Id == job.ToLocationId)
                    .Select(location => location.Code)
                    .Single(),
                CreatedAtUtc = job.CreatedAtUtc,
                UpdatedAtUtc = job.UpdatedAtUtc,
                Steps = job.Steps
                    .OrderBy(step => step.StepNumber)
                    .Select(step => new JobStepView
                    {
                        Id = step.Id,
                        StepNumber = step.StepNumber,
                        Status = step.Status.ToString(),
                        FromLocationCode = dbContext.Locations
                            .Where(location => location.Id == step.FromLocationId)
                            .Select(location => location.Code)
                            .Single(),
                        ToLocationCode = dbContext.Locations
                            .Where(location => location.Id == step.ToLocationId)
                            .Select(location => location.Code)
                            .Single(),
                        CreatedAtUtc = step.CreatedAtUtc,
                        UpdatedAtUtc = step.UpdatedAtUtc
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<Job?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Jobs
            .Include(x => x.Steps)
            .SingleOrDefaultAsync(job => job.Id == id, cancellationToken);
    }

    public async Task AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
