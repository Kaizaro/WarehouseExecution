using Microsoft.EntityFrameworkCore;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure.Jobs.Repositories;

public sealed class LocationRepository(AppDbContext dbContext) : ILocationRepository
{
    public Task<Location?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return dbContext.Locations.SingleOrDefaultAsync(location => location.Code == code, cancellationToken);
    }
}
