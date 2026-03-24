using Microsoft.AspNetCore.Mvc;
using WarehouseExecution.Api.Locations.Controllers;
using WarehouseExecution.Api.Locations.Contracts;
using WarehouseExecution.Application.Locations.Queries;
using Xunit;

namespace WarehouseExecution.Tests.Locations;

public class LocationsControllerTests
{
    [Fact]
    public async Task Get_ReturnsOk_WithLocations()
    {
        var locations = new[]
        {
            new LocationView { Id = Guid.NewGuid(), Code = "A-01", Name = "Inbound Buffer A-01" },
            new LocationView { Id = Guid.NewGuid(), Code = "B-01", Name = "Storage Lane B-01" }
        };

        var controller = new LocationsController(new FakeLocationQueryService(locations));

        var result = await controller.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<IReadOnlyList<LocationResponse>>(okResult.Value);
        Assert.Equal(2, response.Count);
        Assert.Equal("A-01", response[0].Code);
        Assert.Equal("Inbound Buffer A-01", response[0].Name);
    }

    private sealed class FakeLocationQueryService(IReadOnlyList<LocationView> locations) : ILocationQueryService
    {
        public Task<IReadOnlyList<LocationView>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(locations);
        }
    }
}
