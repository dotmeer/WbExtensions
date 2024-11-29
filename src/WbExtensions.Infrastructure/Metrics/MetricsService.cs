using System.Collections.Generic;
using System.Linq;
using WbExtensions.Application.Interfaces.Metrics;

namespace WbExtensions.Infrastructure.Metrics;

internal sealed class MetricsService : IMetricsService
{
    public void IncrementCounter(
        string name,
        IDictionary<string, string>? labels = null,
        string? description = null)
    {
        Prometheus.Metrics.CreateCounter(
                name,
                description ?? name,
                labels?.Keys.ToArray() ?? [])
            .WithLabels(labels?.Values.ToArray() ?? [])
            .Inc();
    }

    public void SetGauge(
        string name,
        double value,
        IDictionary<string, string>? labels = null,
        string? description = null)
    {
        Prometheus.Metrics.CreateGauge(
                name,
                description ?? name,
                labels?.Keys.ToArray() ?? [])
            .WithLabels(labels?.Values.ToArray() ?? [])
            .Set(value);
    }
}