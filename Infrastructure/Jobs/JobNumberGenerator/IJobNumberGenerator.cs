namespace WarehouseExecution.Infrastructure.Jobs;

public interface IJobNumberGenerator
{
    Task<string> NextAsync(CancellationToken cancellationToken = default);
}
