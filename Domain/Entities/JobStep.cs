using WarehouseExecution.Domain.Enums;

namespace WarehouseExecution.Domain.Entities;

public class JobStep
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public int StepNumber { get; set; }
    public JobStepStatus Status { get; set; }
    public Guid ToLocationId { get; set; }
    public Guid FromLocationId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public Job Job { get; set; } = null!;
}
