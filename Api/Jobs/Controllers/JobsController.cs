using Microsoft.AspNetCore.Mvc;
using WarehouseExecution.Application.Jobs.Commands;
using WarehouseExecution.Application.Jobs.Queries;
using WarehouseExecution.Api.Jobs.Contracts;
using WarehouseExecution.Api.Jobs.Routes;
using WarehouseExecution.Api.Jobs.Services;

namespace WarehouseExecution.Api.Jobs.Controllers;

[ApiController]
[Route(JobsRoutes.Base)]
public class JobsController(
    IJobQueryService jobQueryService,
    IJobCommandService jobCommandService,
    IJobExecutionGateway jobExecutionGateway) : ControllerBase
{
    [HttpGet]
    [Route(JobsRoutes.GetAll)]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var jobs = await jobQueryService.GetAllAsync(cancellationToken);
        return Ok(jobs.Select(job => job.ToResponse()).ToList());
    }

    [HttpGet]
    [Route(JobsRoutes.GetById)]
    public async Task<ActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var job = await jobQueryService.GetByIdAsync(id, cancellationToken);

        return job is null ? NotFound() : Ok(job.ToResponse());
    }

    [HttpPost]
    [Route(JobsRoutes.Post)]
    public async Task<ActionResult> Post([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        var job = await jobCommandService.CreateAsync(
            request.FromLocation,
            request.ToLocation,
            request.ProductCode,
            request.ProductName,
            cancellationToken);

        var createdJob = await jobQueryService.GetByIdAsync(job.Id, cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new { id = job.Id },
            createdJob?.ToResponse());
    }

    [HttpPost]
    [Route(JobsRoutes.Execute)]
    public async Task<ActionResult> Execute(Guid id, CancellationToken cancellationToken)
    {
        var response = await jobExecutionGateway.ExecuteAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Route(JobsRoutes.Cancel)]
    public async Task<ActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var response = await jobExecutionGateway.CancelAsync(id, cancellationToken);
        return Ok(response);
    }
}
