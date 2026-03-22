using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using WarehouseExecution.Api.Jobs.Contracts;
using WarehouseExecution.Api.Jobs.Routes;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Infrastructure.Jobs;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Api.Jobs.Controllers;

[ApiController]
[Route(JobsRoutes.Base)]
public class JobsController(IJobNumberGenerator jobNumberGenerator, AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [Route(JobsRoutes.GetAll)]
    public ActionResult Get()
    {
        return Ok();
    }

    [HttpGet]
    [Route(JobsRoutes.GetById)]
    public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs
            .Include(x => x.Steps)
            .SingleOrDefaultAsync(jobEntity => jobEntity.Id == id, cancellationToken);

        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    [Route(JobsRoutes.Post)]
    public async Task<ActionResult> Post([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = await jobNumberGenerator.NextAsync(cancellationToken),
            Status = JobStatus.Created,
            FromLocation = request.FromLocation,
            ToLocation = request.ToLocation,
            ProductCode = request.ProductCode,
            ProductName = request.ProductName
        };

        dbContext.Jobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);

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
