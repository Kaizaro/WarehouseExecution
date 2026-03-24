using WarehouseExecution.Application.Common;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Application.Jobs.Commands;
using WarehouseExecution.Domain.Entities;
using Xunit;

namespace WarehouseExecution.Tests.Jobs;

public class JobCommandServiceTests
{
    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenSourceLocationIsEmpty()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync("   ", "B-01", "SKU-1", "Box", CancellationToken.None));

        Assert.Equal("Source location is required.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenLocationsAreTheSame()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync(" A-01 ", "a-01", "SKU-1", "Box", CancellationToken.None));

        Assert.Equal("Source and destination locations must be different.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_ThrowsNotFound_WhenSourceLocationDoesNotExist()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync("Z-99", "B-01", "SKU-1", "Box", CancellationToken.None));

        Assert.Equal("Source location 'Z-99' was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_CreatesJobWithResolvedLocationIds()
    {
        var sourceLocation = new Location
        {
            Id = Guid.NewGuid(),
            Code = "A-01",
            Name = "Inbound Buffer A-01"
        };
        var destinationLocation = new Location
        {
            Id = Guid.NewGuid(),
            Code = "B-01",
            Name = "Storage Lane B-01"
        };

        var repository = new FakeJobRepository();
        var service = CreateService(repository, sourceLocation, destinationLocation);

        var job = await service.CreateAsync("A-01", "B-01", "SKU-1", "Box", CancellationToken.None);

        Assert.Equal(sourceLocation.Id, job.FromLocationId);
        Assert.Equal(destinationLocation.Id, job.ToLocationId);
        Assert.Same(job, repository.AddedJob);
    }

    private static JobCommandService CreateService(
        FakeJobRepository? repository = null,
        params Location[] locations)
    {
        repository ??= new FakeJobRepository();
        if (locations.Length == 0)
        {
            locations =
            [
                new Location { Id = Guid.NewGuid(), Code = "A-01", Name = "Inbound Buffer A-01" },
                new Location { Id = Guid.NewGuid(), Code = "B-01", Name = "Storage Lane B-01" }
            ];
        }

        return new JobCommandService(
            repository,
            new FakeJobNumberGenerator(),
            new FakeLocationRepository(locations));
    }

    private sealed class FakeJobRepository : IJobRepository
    {
        public Job? AddedJob { get; private set; }

        public Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task AddAsync(Job job, CancellationToken cancellationToken = default)
        {
            AddedJob = job;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeJobNumberGenerator : IJobNumberGenerator
    {
        public Task<string> NextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult("JOB-20260324-000001");
        }
    }

    private sealed class FakeLocationRepository(IEnumerable<Location> locations) : ILocationRepository
    {
        public Task<Location?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(locations.SingleOrDefault(location => location.Code == code));
        }
    }
}
