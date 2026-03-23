using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Infrastructure.Jobs.Execution;

public interface IJobExecutionService
{
    Task<Job> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default);
}
