using WarehouseExecution.Application.Common;

namespace WarehouseExecution.Api.Common;

public sealed class ApplicationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteAsync(context, StatusCodes.Status400BadRequest, ApiErrorResponse.BadRequest(exception.Message));
        }
        catch (NotFoundException exception)
        {
            await WriteAsync(context, StatusCodes.Status404NotFound, ApiErrorResponse.NotFound(exception.Message));
        }
        catch (ConflictException exception)
        {
            await WriteAsync(context, StatusCodes.Status409Conflict, ApiErrorResponse.Conflict(exception.Message));
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, ApiErrorResponse response)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}
