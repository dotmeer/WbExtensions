using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using WbExtensions.Application.Interfaces.Metrics;

namespace WbExtensions.Infrastructure.Metrics;

internal sealed class MetricsService : IMetricsService
{
    private readonly Meter _customMetrics = new("WbExtensions.Metrics");

    internal string MeterName => _customMetrics.Name;

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

        _customMetrics.CreateCounter<int>(
                name,
                unit: null,
                description ?? name)
            .Add(1, labels?.Select(label => new KeyValuePair<string, object?>(label.Key, label.Value)).ToArray()
                    ?? []);
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

        _customMetrics.CreateObservableGauge(
            name,
            () => new Measurement<double>(
                value,
                labels?.Select(label => new KeyValuePair<string, object?>(label.Key, label.Value)).ToArray()
                ?? []),
            unit: null,
            description ?? null);
    }
}