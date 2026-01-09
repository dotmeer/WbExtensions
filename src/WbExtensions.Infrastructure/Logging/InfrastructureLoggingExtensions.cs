using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace WbExtensions.Infrastructure.Logging;

public static class InfrastructureLoggingExtensions
{
    private static ILoggingBuilder AddMetricsLogger(
        this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, MetricsLoggerProvider>());

        return builder;
    }

    public static WebApplicationBuilder SetupLogging(
        this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddJsonConsole(_ =>
        {
            _.TimestampFormat = "O";
            _.UseUtcTimestamp = true;
            _.IncludeScopes = true;
            _.JsonWriterOptions = new JsonWriterOptions
            {
                Indented = false
            };
        });
        builder.Logging.AddMetricsLogger();

        return builder;
    }
}