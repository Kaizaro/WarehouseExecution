using System.Text.Json;
using Microsoft.AspNetCore.Http;
using WarehouseExecution.Api.Common;
using WarehouseExecution.Application.Common;
using Xunit;

namespace WarehouseExecution.Tests.Api;

public class ApplicationExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Returns400_ForValidationException()
    {
        var response = await InvokeAsync(new ValidationException("Source location is required."));

        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Contains("BAD_REQUEST", response.Body);
    }

    [Fact]
    public async Task InvokeAsync_Returns404_ForNotFoundException()
    {
        var response = await InvokeAsync(new NotFoundException("Location not found."));

        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        Assert.Contains("NOT_FOUND", response.Body);
    }

    [Fact]
    public async Task InvokeAsync_Returns409_ForConflictException()
    {
        var response = await InvokeAsync(new ConflictException("Job cannot be executed."));

        Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        Assert.Contains("CONFLICT", response.Body);
    }

    private static async Task<(int StatusCode, string Body)> InvokeAsync(Exception exception)
    {
        var middleware = new ApplicationExceptionMiddleware(_ => throw exception);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        JsonDocument.Parse(body);

        return (context.Response.StatusCode, body);
    }
}
