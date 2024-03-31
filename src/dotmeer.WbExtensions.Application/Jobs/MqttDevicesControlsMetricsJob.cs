using System;
using System.Collections.Generic;
using dotmeer.WbExtensions.Infrastructure.Metrics.Abstractions;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

namespace dotmeer.WbExtensions.Application.Jobs;

public sealed class MqttDevicesControlsMetricsJob : IJob
{
    private readonly ILogger<MqttDevicesControlsMetricsJob> _logger;

    private readonly IMetricsService _metricsService;

    private readonly IMqttService _mqttService;

    public MqttDevicesControlsMetricsJob(
        ILogger<MqttDevicesControlsMetricsJob> logger,
        IMetricsService metricsService,
        IMqttService mqttService)
    {
        _logger = logger;
        _metricsService = metricsService;
        _mqttService = mqttService;
    }

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _mqttService.SubscribeAsync(
            QueueConnection.WirenBoard("/devices/+/controls/+", "prometheus"),
            (message, ct) =>
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
            },
            stoppingToken);
    }
}