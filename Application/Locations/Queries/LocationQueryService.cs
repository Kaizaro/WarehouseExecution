using WarehouseExecution.Application.Jobs.Abstractions;

namespace WarehouseExecution.Application.Locations.Queries;

public sealed class LocationQueryService(ILocationRepository locationRepository) : ILocationQueryService
{
    public Task<IReadOnlyList<LocationView>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return locationRepository.GetAllAsync(cancellationToken);
    }
}
