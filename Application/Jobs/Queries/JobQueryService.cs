using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Application.Jobs.Queries;

public sealed class JobQueryService(IJobRepository jobRepository) : IJobQueryService
{
    public Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return jobRepository.GetAllAsync(cancellationToken);
    }

    public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return jobRepository.GetByIdAsync(id, cancellationToken);
    }
}
