using dotmeer.WbExtensions.Infrastructure.Metrics.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace dotmeer.WbExtensions.Infrastructure.Logging;

internal sealed class MetricsLogger : ILogger
{
    private readonly IMetricsService _metricsService;

    public MetricsLogger(IMetricsService metricsService)
    {
        _metricsService = metricsService;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception, string> formatter)
    {
        _metricsService.IncrementCounter(
            "log_event",
            new Dictionary<string, string>
            {
                ["level"] = logLevel.ToString()
            },
            "Log event");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => default;
}