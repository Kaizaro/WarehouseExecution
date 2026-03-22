using Domain.Enums;

namespace WarehouseExecution.Domain.Entities;

public class Job
{
    public Guid Id { get; set; }
    public required string JobNumber { get; set; }
    public JobStatus Status { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public required string ToLocation { get; set; }
    public required string FromLocation { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public ICollection<JobStep> Steps { get; set; } = [];
}
