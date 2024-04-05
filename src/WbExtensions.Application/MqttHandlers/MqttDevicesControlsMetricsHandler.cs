using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WbExtensions.Infrastructure.Metrics.Abstractions;
using WbExtensions.Infrastructure.Mqtt.Abstractions;

namespace WbExtensions.Application.MqttHandlers;

public sealed class MqttDevicesControlsMetricsHandler : IMqttHandler
{
    private readonly ILogger<MqttDevicesControlsMetricsHandler> _logger;
    private readonly IMetricsService _metricsService;

    public MqttDevicesControlsMetricsHandler(
        ILogger<MqttDevicesControlsMetricsHandler> logger, 
        IMetricsService metricsService)
    {
        _logger = logger;
        _metricsService = metricsService;
    }

    public Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var topic = message.Topic.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var deviceName = topic[1];
            var controlName = topic[3];

            if (double.TryParse(message.Payload, out var value))
            {
                _metricsService.SetGauge(
                    "mqtt_topic_values",
                    value,
                    new Dictionary<string, string>
                    {
                        ["device_name"] = deviceName,
                        ["control_name"] = controlName
                    },
                    "Values from mqtt topics");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while reading device controls");
        }

        return Task.CompletedTask;
    }
}