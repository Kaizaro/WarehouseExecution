namespace WarehouseExecution.Worker.Services;

public interface IExecutionScheduler
{
    void Schedule(Guid jobId);
    bool TryCancel(Guid jobId);
}
