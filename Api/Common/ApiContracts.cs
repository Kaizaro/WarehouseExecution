namespace WarehouseExecution.Api.Common;

public sealed class ApiSuccess
{
    public string? Status { get; init; }
    public string? Code { get; init; }
}

public sealed class ApiSuccess<TData>
{
    public string? Status { get; init; }
    public string? Code { get; init; }
    public TData? Data { get; init; }
}

public sealed class ApiError
{
    public string? Status { get; init; }
    public string? Code { get; init; }
    public string? Title { get; init; }
    public string? Detail { get; init; }
}

public sealed class ApiSuccessResponse
{
    private const string SuccessStatus = "OK";
    private const string SuccessCode = "200";
    private const string CreatedAtStatus = "CreatedAt";
    private const string CreatedAtCode = "201";

    public static ApiSuccess Success() =>
        new()
        {
            Status = SuccessStatus,
            Code = SuccessCode,
        };

    public static ApiSuccess<TData> Success<TData>(TData data) =>
        new()
        {
            Status = SuccessStatus,
            Code = SuccessCode,
            Data = data
        };

    public static ApiSuccess CreatedAt() =>
        new()
        {
            Status = CreatedAtStatus,
            Code = CreatedAtCode,
        };
    
    public static ApiSuccess<TData> CreatedAt<TData>(TData data) =>
        new()
        {
            Status = CreatedAtStatus,
            Code = CreatedAtCode,
            Data = data
        };
}

public sealed class ApiErrorResponse
{
    public List<ApiError> Errors { get; init; } = [];

    public static ApiErrorResponse BadRequest(string detail) =>
        new()
        {
            Errors =
            [
                new ApiError
                {
                    Status = "400",
                    Code = "BAD_REQUEST",
                    Title = "Bad Request",
                    Detail = detail
                }
            ]
        };

    public static ApiErrorResponse Unauthorized(string detail) =>
        new()
        {
            Errors =
            [
                new ApiError
                {
                    Status = "401",
                    Code = "UNAUTHORIZED",
                    Title = "Unauthorized",
                    Detail = detail
                }
            ]
        };

    public static ApiErrorResponse NotFound(string detail) =>
        new()
        {
            Errors =
            [
                new ApiError
                {
                    Status = "404",
                    Code = "NOT_FOUND",
                    Title = "Not Found",
                    Detail = detail
                }
            ]
        };

    public static ApiErrorResponse Forbidden(string detail) =>
        new()
        {
            Errors =
            [
                new ApiError
                {
                    Status = "403",
                    Code = "FORBIDDEN",
                    Title = "Forbidden",
                    Detail = detail
                }
            ]
        };

    public static ApiErrorResponse Conflict(string detail) =>
        new()
        {
            Errors =
            [
                new ApiError
                {
                    Status = "409",
                    Code = "CONFLICT",
                    Title = "Conflict",
                    Detail = detail
                }
            ]
        };
}
