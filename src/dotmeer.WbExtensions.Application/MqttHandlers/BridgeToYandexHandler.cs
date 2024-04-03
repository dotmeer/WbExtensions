using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dotmeer.WbExtensions.Infrastructure.Metrics.Abstractions;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;

namespace dotmeer.WbExtensions.Application.MqttHandlers;

public sealed class BridgeToYandexHandler : IMqttHandler
{
    private readonly IMqttService _mqttService;
    private readonly IMetricsService _metricsService;
    private readonly IDictionary<string, string?> _cachedValues;
    private readonly HashSet<string> _allowedDevices;
    private readonly HashSet<string> _allowedControls;

    public BridgeToYandexHandler(
        IMqttService mqttService,
        IMetricsService metricsService)
    {
        _mqttService = mqttService;
        _metricsService = metricsService;
        _cachedValues = new ConcurrentDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        _allowedDevices = InitAllowedDevices();
        _allowedControls = InitAllowedControls();
    }

    public async Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var topic = message.Topic.Split("/", StringSplitOptions.RemoveEmptyEntries);
        var deviceName = topic[1];
        var controlName = topic[3];

        if (!_allowedDevices.Contains(deviceName)
            || !_allowedControls.Contains(controlName))
        {
            return;
        }

        if (_cachedValues.TryGetValue(message.Topic, out var value)
            && value == message.Payload)
        {
            return;
        }

        _cachedValues[message.Topic] = message.Payload;

        await _mqttService.PublishAsync(
            QueueConnection.Yandex(message.Topic, "wb2yandex"),
            message.Payload,
            cancellationToken);

        _metricsService.IncrementCounter(
            "publish_to_yandex",
            description: "Messages published to yandex IoT/");
    }

    private HashSet<string> InitAllowedDevices()
    {
        return new HashSet<string>
        {
            "wb-mr6c_26",
            "wb-mr6cu_84",
            "wb-mr6c_15",
            "aquastop",
            "sensor-hall",
            "sensor-front-door",
            "sensor-balcony",
            "sensor-kitchen",
            "sensor-toilet",
            "sensor-bathroom",
            "sensor-bedroom",
            "sensor-livingroom",
            "blind-motor-left-livingroom",
            "blind-motor-right-livingroom",
            "blind-motor-left-bedroom",
            "blind-motor-right-bedroom",
            "wc-fan"
        };
    }

    private HashSet<string> InitAllowedControls()
    {
        return new HashSet<string>
        {
            "state",
            "position",
            "temperature",
            "humidity",
            "voc",
            "co2",
            "illuminance_lux",
            "noise",
            "occupancy",
            "contact",
            "K1",
            "K2",
            "K3",
            "K4",
            "K5",
            "K6",
            "Input 1",
            "Input 2",
            "enabled"
        };
    }
}