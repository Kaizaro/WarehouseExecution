using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Application.Jobs.Abstractions;

public interface IJobRepository
{
    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
