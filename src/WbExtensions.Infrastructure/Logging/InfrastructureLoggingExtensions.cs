using Microsoft.AspNetCore.Hosting;
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
    
    public static IWebHostBuilder SetupLogging(
        this IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.AddMetricsLogger();
        });

        return builder;
    }
}