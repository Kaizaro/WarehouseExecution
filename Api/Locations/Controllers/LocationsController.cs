using Microsoft.AspNetCore.Mvc;
using WarehouseExecution.Api.Locations.Contracts;
using WarehouseExecution.Api.Locations.Routes;
using WarehouseExecution.Application.Locations.Queries;

namespace WarehouseExecution.Api.Locations.Controllers;

[ApiController]
[Route(LocationsRoutes.Base)]
public class LocationsController(ILocationQueryService locationQueryService) : ControllerBase
{
    [HttpGet]
    [Route(LocationsRoutes.GetAll)]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var locations = await locationQueryService.GetAllAsync(cancellationToken);
        return Ok(locations.Select(location => location.ToResponse()).ToList());
    }
}
