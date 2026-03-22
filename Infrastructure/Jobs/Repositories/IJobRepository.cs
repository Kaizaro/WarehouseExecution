using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public interface IJobRepository
{
    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Job> CreateAsync(
        string fromLocation,
        string toLocation,
        string? productCode,
        string? productName,
        CancellationToken cancellationToken = default);
}
