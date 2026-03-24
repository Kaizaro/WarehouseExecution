namespace WarehouseExecution.Application.Jobs.Queries;

public sealed class JobStepView
{
    public Guid Id { get; init; }
    public int StepNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public string FromLocationCode { get; init; } = string.Empty;
    public string ToLocationCode { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
