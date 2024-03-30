using dotmeer.WbExtensions.Infrastructure.Metrics.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace dotmeer.WbExtensions.Infrastructure.Logging;

internal sealed class MetricsLoggerProvider : ILoggerProvider
{
    private readonly IMetricsService _metricsService;

    private readonly ConcurrentDictionary<string, ILogger> _loggers;

    public MetricsLoggerProvider(IMetricsService metricsService)
    {
        _metricsService = metricsService;
        _loggers = new ConcurrentDictionary<string, ILogger>();
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, new MetricsLogger(_metricsService));

    public void Dispose()
        => _loggers.Clear();
}