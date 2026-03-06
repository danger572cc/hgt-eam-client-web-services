using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HGT.EAM.Client.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static ILoggerFactory CreateSerilogLoggerFactory(IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console();

        var logPath = configuration["Serilog:WriteTo:File:Path"];
        if (!string.IsNullOrWhiteSpace(logPath))
        {
            loggerConfig.WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        var logger = loggerConfig.CreateLogger();
        return new LoggerFactory().AddSerilog(logger);
    }
}
