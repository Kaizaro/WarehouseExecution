using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Application.Jobs.Queries;

public interface IJobQueryService
{
    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
