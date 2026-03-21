using Domain.Enums;

namespace Domain.Entities;

public class Job
{
    public Guid Id { get; set; }
    public string JobNumber { get; set; }
    public JobStatus Status { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ToLocation { get; set; }
    public string FromLocation { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}