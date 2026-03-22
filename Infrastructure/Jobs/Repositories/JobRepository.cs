using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Jobs;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public sealed class JobRepository(AppDbContext dbContext, IJobNumberGenerator jobNumberGenerator) : IJobRepository
{
    public async Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .Include(x => x.Steps)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Jobs
            .Include(x => x.Steps)
            .SingleOrDefaultAsync(job => job.Id == id, cancellationToken);
    }

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

        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);

        return job;
    }
}
