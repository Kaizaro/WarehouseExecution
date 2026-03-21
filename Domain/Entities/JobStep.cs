using Domain.Enums;

namespace Domain.Entities;

public class JobStep
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public int StepNumber { get; set; }
    public JobStatus Status { get; set; }
    public string ToLocation { get; set; }
    public string FromLocation { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}