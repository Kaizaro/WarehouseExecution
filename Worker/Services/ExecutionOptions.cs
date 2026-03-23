namespace WarehouseExecution.Worker.Services;

public sealed class ExecutionOptions
{
    public int MinDelaySeconds { get; set; } = 60;
    public int MaxDelaySeconds { get; set; } = 120;
}
