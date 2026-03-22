namespace WarehouseExecution.Api.Jobs.Contracts;

public class CreateJobRequest
{
    public required string FromLocation { get; set; }
    public required string ToLocation { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
}
