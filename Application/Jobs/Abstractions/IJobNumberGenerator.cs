namespace WarehouseExecution.Application.Jobs.Abstractions;

public interface IJobNumberGenerator
{
    Task<string> NextAsync(CancellationToken cancellationToken = default);
}
