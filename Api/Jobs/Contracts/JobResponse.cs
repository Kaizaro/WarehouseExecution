namespace WarehouseExecution.Api.Jobs.Contracts;

public sealed class JobResponse
{
    public Guid Id { get; init; }
    public string JobNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ProductCode { get; init; }
    public string? ProductName { get; init; }
    public string FromLocation { get; init; } = string.Empty;
    public string ToLocation { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public IReadOnlyList<JobStepResponse> Steps { get; init; } = [];
}
