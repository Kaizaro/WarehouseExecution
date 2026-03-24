namespace WarehouseExecution.Application.Locations.Queries;

public interface ILocationQueryService
{
    Task<IReadOnlyList<LocationView>> GetAllAsync(CancellationToken cancellationToken = default);
}
