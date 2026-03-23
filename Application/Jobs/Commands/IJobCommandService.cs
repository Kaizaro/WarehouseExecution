using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Application.Jobs.Commands;

public interface IJobCommandService
{
    Task<Job> CreateAsync(
        string fromLocation,
        string toLocation,
        string? productCode,
        string? productName,
        CancellationToken cancellationToken = default);

    Task<Job> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default);
}
