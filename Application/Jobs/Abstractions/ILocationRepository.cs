using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Application.Locations.Queries;

namespace WarehouseExecution.Application.Jobs.Abstractions;

public interface ILocationRepository
{
    Task<Location?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocationView>> GetAllAsync(CancellationToken cancellationToken = default);
}
