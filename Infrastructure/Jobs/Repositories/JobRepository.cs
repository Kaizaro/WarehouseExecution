using Microsoft.EntityFrameworkCore;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public sealed class JobRepository(AppDbContext dbContext) : IJobRepository
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
