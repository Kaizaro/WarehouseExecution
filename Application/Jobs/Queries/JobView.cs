namespace WarehouseExecution.Application.Jobs.Queries;

public sealed class JobView
{
    public Guid Id { get; init; }
    public string JobNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ProductCode { get; init; }
    public string? ProductName { get; init; }
    public string FromLocationCode { get; init; } = string.Empty;
    public string ToLocationCode { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public IReadOnlyList<JobStepView> Steps { get; init; } = [];
}
