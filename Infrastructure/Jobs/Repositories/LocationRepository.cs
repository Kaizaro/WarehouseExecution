using Microsoft.EntityFrameworkCore;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Application.Locations.Queries;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public sealed class LocationRepository(AppDbContext dbContext) : ILocationRepository
{
    public Task<Location?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return dbContext.Locations.SingleOrDefaultAsync(location => location.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<LocationView>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Locations
            .OrderBy(location => location.Code)
            .Select(location => new LocationView
            {
                Id = location.Id,
                Code = location.Code,
                Name = location.Name,
                CreatedAtUtc = location.CreatedAtUtc,
                UpdatedAtUtc = location.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
