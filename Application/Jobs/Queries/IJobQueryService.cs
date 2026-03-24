namespace WarehouseExecution.Application.Jobs.Queries;

public interface IJobQueryService
{
    Task<IReadOnlyList<JobView>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<JobView?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
