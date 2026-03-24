using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Application.Jobs.Queries;

namespace WarehouseExecution.Application.Jobs.Abstractions;

public interface IJobRepository
{
    Task<IReadOnlyList<JobView>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<JobView?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Job?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
