using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace WarehouseExecution.Infrastructure.Logging;

public class SerilogConfigurator
{
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

    public static ILogger CreateLogger(IConfiguration configuration, string applicationName)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.FromLogContext()
            .Enrich.With(new JstTimestampEnricher())
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
            .MinimumLevel.Override("Npgsql", LogEventLevel.Error)
            .WriteTo.Console(outputTemplate:
                "[{JstTimestamp} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Map(
                le => le.Timestamp.ToOffset(JstOffset).ToString("yyyyMMdd_HH00"),
                (hourKey, wt) =>
                {
                    var dateFolder = hourKey.Substring(0, 8);
                    var folder = Path.Combine("Logs", dateFolder);
                    Directory.CreateDirectory(folder);
                    var filePath = Path.Combine(folder, $"{hourKey}.log");
                    wt.File(
                        filePath,
                        rollingInterval: RollingInterval.Infinite,
                        outputTemplate: "[{JstTimestamp} {Level:u3}] [{Application}] {Message:lj}{NewLine}{Exception}");
                }
            )
            .CreateLogger();

        return logger;
    }

    private sealed class JstTimestampEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string jstTimestamp = logEvent.Timestamp
                .ToOffset(JstOffset)
                .ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("JstTimestamp", jstTimestamp));
        }
    }
}