namespace WarehouseExecution.Api.Jobs.Routes;

public static class JobsRoutes
{
    public const string Base = "jobs";
    public const string GetAll = "";
    public const string GetById = "{id}";
    public const string Post = "";
    public const string Execute = "{id}/actions/execute";
    public const string Cancel = "{id}/actions/cancel";
}