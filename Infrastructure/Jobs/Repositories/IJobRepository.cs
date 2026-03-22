using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public interface IJobRepository
{
    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
}
