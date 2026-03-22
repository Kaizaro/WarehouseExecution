namespace WarehouseExecution.Infrastructure.Persistence;

internal sealed class JobNumberCounter
{
    public DateOnly Date { get; set; }
    public int LastValue { get; set; }
}
