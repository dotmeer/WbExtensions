using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Metrics;
using WbExtensions.Application.Models.Notification;

namespace WbExtensions.Application.Handlers.Notification;

internal sealed class MetricsHandler : INotificationHandler<ValueNotification>
{
    private readonly IMetricsService _metricsService;
    private readonly ILogger<MetricsHandler> _logger;

    public MetricsHandler(
        IMetricsService metricsService,
        ILogger<MetricsHandler> logger)
    {
        _metricsService = metricsService;
        _logger = logger;
    }

    public Task Handle(ValueNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            if (double.TryParse(notification.Value, out var doubleValue))
            {
                _metricsService.SetGauge(
                    "mqtt_topic_values",
                    doubleValue,
                    new Dictionary<string, string>
                    {
                        ["device_name"] = notification.VirtualDeviceName,
                        ["control_name"] = notification.VirtualControlName
                    },
                    "Values from mqtt topics");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while saving telemetry for device '{Device}' and control '{Control}'",
                notification.VirtualDeviceName,
                notification.VirtualControlName);
        }

        return Task.CompletedTask;
    }
}