namespace WarehouseExecution.Api.Jobs.Contracts;

public sealed class JobStepResponse
{
    public Guid Id { get; init; }
    public int StepNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public string FromLocation { get; init; } = string.Empty;
    public string ToLocation { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
