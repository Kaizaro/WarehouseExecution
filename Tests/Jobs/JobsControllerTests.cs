using Microsoft.AspNetCore.Mvc;
using WarehouseExecution.Api.Jobs.Contracts;
using WarehouseExecution.Api.Jobs.Controllers;
using WarehouseExecution.Application.Jobs.Commands;
using WarehouseExecution.Application.Jobs.Queries;
using WarehouseExecution.Domain.Entities;
using WarehouseExecution.Domain.Enums;
using Xunit;

namespace WarehouseExecution.Tests.Jobs;

public class JobsControllerTests
{
    [Fact]
    public async Task Get_ReturnsOk_WithJobs()
    {
        var expectedJobs = new List<Job>
        {
            CreateJob("JOB-20260323-000001"),
            CreateJob("JOB-20260323-000002")
        };

        var controller = new JobsController(
            new FakeJobQueryService(expectedJobs),
            new FakeJobCommandService(CreateJob("JOB-20260323-999999")));

        var result = await controller.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var jobs = Assert.IsAssignableFrom<IReadOnlyList<Job>>(okResult.Value);
        Assert.Equal(2, jobs.Count);
        Assert.Equal(expectedJobs[0].Id, jobs[0].Id);
        Assert.Equal(expectedJobs[1].Id, jobs[1].Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenJobDoesNotExist()
    {
        var controller = new JobsController(
            new FakeJobQueryService([]),
            new FakeJobCommandService(CreateJob("JOB-20260323-999999")));

        var result = await controller.Get(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtAction_WithCreatedJob()
    {
        var createdJob = CreateJob("JOB-20260323-000123");
        var commandService = new FakeJobCommandService(createdJob);
        var controller = new JobsController(new FakeJobQueryService([]), commandService);
        var request = new CreateJobRequest
        {
            FromLocation = "A-01",
            ToLocation = "B-02",
            ProductCode = "SKU-1",
            ProductName = "Box"
        };

        var result = await controller.Post(request, CancellationToken.None);

        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(JobsController.Get), createdAtResult.ActionName);
        Assert.Equal(createdJob.Id, createdAtResult.RouteValues!["id"]);

        var responseJob = Assert.IsType<Job>(createdAtResult.Value);
        Assert.Equal(createdJob.Id, responseJob.Id);
        Assert.Equal(request.FromLocation, commandService.LastFromLocation);
        Assert.Equal(request.ToLocation, commandService.LastToLocation);
        Assert.Equal(request.ProductCode, commandService.LastProductCode);
        Assert.Equal(request.ProductName, commandService.LastProductName);
    }

    private static Job CreateJob(string jobNumber)
    {
        return new Job
        {
            Id = Guid.NewGuid(),
            JobNumber = jobNumber,
            Status = JobStatus.Created,
            FromLocation = "SRC-01",
            ToLocation = "DST-01"
        };
    }

    private sealed class FakeJobQueryService(IReadOnlyList<Job> jobs) : IJobQueryService
    {
        public Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(jobs);
        }

        public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(jobs.SingleOrDefault(job => job.Id == id));
        }
    }

    private sealed class FakeJobCommandService(Job createdJob) : IJobCommandService
    {
        public string? LastFromLocation { get; private set; }
        public string? LastToLocation { get; private set; }
        public string? LastProductCode { get; private set; }
        public string? LastProductName { get; private set; }

        public Task<Job> CreateAsync(
            string fromLocation,
            string toLocation,
            string? productCode,
            string? productName,
            CancellationToken cancellationToken = default)
        {
            LastFromLocation = fromLocation;
            LastToLocation = toLocation;
            LastProductCode = productCode;
            LastProductName = productName;

            return Task.FromResult(createdJob);
        }

        public Task<Job> ExecuteAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
