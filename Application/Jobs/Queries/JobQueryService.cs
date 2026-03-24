using WarehouseExecution.Application.Jobs.Abstractions;
namespace WarehouseExecution.Application.Jobs.Queries;

public sealed class JobQueryService(IJobRepository jobRepository) : IJobQueryService
{
    public Task<IReadOnlyList<JobView>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return jobRepository.GetAllAsync(cancellationToken);
    }

    public Task<JobView?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return jobRepository.GetByIdAsync(id, cancellationToken);
    }
}
