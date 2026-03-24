using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Application.Jobs.Abstractions;

public interface ILocationRepository
{
    Task<Location?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
