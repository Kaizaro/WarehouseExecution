using System.Data;
using Microsoft.EntityFrameworkCore;
using WarehouseExecution.Application.Jobs.Abstractions;
using WarehouseExecution.Infrastructure.Persistence;

namespace WarehouseExecution.Infrastructure.Jobs.JobNumberGenerator;

public sealed class DbJobNumberGenerator(AppDbContext dbContext) : IJobNumberGenerator
{
    public async Task<string> NextAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var connection = dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO "JobNumberCounters" ("Date", "LastValue")
                VALUES (@date, 1)
                ON CONFLICT ("Date")
                DO UPDATE SET "LastValue" = "JobNumberCounters"."LastValue" + 1
                RETURNING "LastValue";
                """;

            var dateParameter = command.CreateParameter();
            dateParameter.ParameterName = "date";
            dateParameter.Value = today;
            command.Parameters.Add(dateParameter);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var nextValue = Convert.ToInt32(result);

            return $"JOB-{today:yyyyMMdd}-{nextValue:D6}";
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }
}
