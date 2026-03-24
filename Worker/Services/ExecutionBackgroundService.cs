using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using WarehouseExecution.Application.Common;
using WarehouseExecution.Application.Jobs.Commands;

namespace WarehouseExecution.Worker.Services;

public sealed class ExecutionBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<ExecutionOptions> options) : BackgroundService, IExecutionScheduler
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _cancellations = new();
    private readonly ExecutionOptions _options = options.Value;
    private readonly Random _random = new();

    public void Schedule(Guid jobId)
    {
        _channel.Writer.TryWrite(jobId);
    }

    public bool TryCancel(Guid jobId)
    {
        if (_cancellations.TryGetValue(jobId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            return true;
        }

        return false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var jobId in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            _ = ProcessJobAsync(jobId, stoppingToken);
        }
    }

    private async Task ProcessJobAsync(Guid jobId, CancellationToken stoppingToken)
    {
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        if (!_cancellations.TryAdd(jobId, cancellationTokenSource))
        {
            return;
        }

        try
        {
            await using var startScope = serviceScopeFactory.CreateAsyncScope();
            var startCommandService = startScope.ServiceProvider.GetRequiredService<IJobCommandService>();
            await startCommandService.StartExecutionAsync(jobId, cancellationTokenSource.Token);

            var delaySeconds = _random.Next(_options.MinDelaySeconds, _options.MaxDelaySeconds + 1);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationTokenSource.Token);

            await using var completeScope = serviceScopeFactory.CreateAsyncScope();
            var completeCommandService = completeScope.ServiceProvider.GetRequiredService<IJobCommandService>();
            await completeCommandService.CompleteAsync(jobId, CancellationToken.None);
        }
        catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
        {
            await using var cancelScope = serviceScopeFactory.CreateAsyncScope();
            var cancelCommandService = cancelScope.ServiceProvider.GetRequiredService<IJobCommandService>();
            await cancelCommandService.CancelAsync(jobId, CancellationToken.None);
        }
        catch (AppException)
        {
        }
        finally
        {
            _cancellations.TryRemove(jobId, out _);
        }
    }
}
