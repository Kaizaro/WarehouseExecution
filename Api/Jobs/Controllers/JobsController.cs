using Microsoft.AspNetCore.Mvc;
using WarehouseExecution.Api.Jobs.Contracts;
using WarehouseExecution.Api.Jobs.Routes;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Jobs.Repositories;

namespace WarehouseExecution.Api.Jobs.Controllers;

[ApiController]
[Route(JobsRoutes.Base)]
public class JobsController(IJobRepository jobRepository) : ControllerBase
{
    [HttpGet]
    [Route(JobsRoutes.GetAll)]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var jobs = await jobRepository.GetAllAsync(cancellationToken);
        return Ok(jobs);
    }

    [HttpGet]
    [Route(JobsRoutes.GetById)]
    public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var job = await jobRepository.GetByIdAsync(id, cancellationToken);

        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    [Route(JobsRoutes.Post)]
    public async Task<ActionResult> Post([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        var job = await jobRepository.CreateAsync(
            request.FromLocation,
            request.ToLocation,
            request.ProductCode,
            request.ProductName,
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = job.Id }, job);
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
