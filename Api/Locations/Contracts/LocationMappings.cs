using WarehouseExecution.Application.Locations.Queries;

namespace WarehouseExecution.Api.Locations.Contracts;

public static class LocationMappings
{
    public static LocationResponse ToResponse(this LocationView location)
    {
        return new LocationResponse
        {
            Id = location.Id,
            Code = location.Code,
            Name = location.Name,
            CreatedAtUtc = location.CreatedAtUtc,
            UpdatedAtUtc = location.UpdatedAtUtc
        };
    }
}
