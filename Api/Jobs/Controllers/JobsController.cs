using Microsoft.AspNetCore.Mvc;
using WarehouseExecution.Api.Jobs.Routes;
using WarehouseExecution.Domain.Entities;

namespace WarehouseExecution.Api.Jobs.Controllers;

[ApiController]
[Route(JobsRoutes.Base)]
public class JobsController : ControllerBase
{
    [HttpGet]
    [Route(JobsRoutes.GetAll)]
    public ActionResult Get()
    {
        return Ok();
    }

    [HttpGet]
    [Route(JobsRoutes.GetById)]
    public ActionResult Get(int id)
    {
        return Ok();
    }
    
    [HttpPost]
    [Route(JobsRoutes.Post)]
    public ActionResult Post([FromBody] Job job)
    {
        return Created();
    }

    [HttpPost]
    [Route(JobsRoutes.Execute)]
    public ActionResult Execute([FromBody] Job job)
    {
        return Ok();
    }

    [HttpPost]
    [Route(JobsRoutes.Cancel)]
    public ActionResult Cancel([FromBody] Job job)
    {
        return Ok();
    }
}