using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;

namespace dotmeer.WbExtensions.Application.Jobs;

public sealed class LogZigbee2MqttEventsJob : IJob
{
    private readonly ILogger<LogZigbee2MqttEventsJob> _logger;

    private readonly IMqttService _mqttService;

    public LogZigbee2MqttEventsJob(
        ILogger<LogZigbee2MqttEventsJob> logger,
        IMqttService mqttService)
    {
        _logger = logger;
        _mqttService = mqttService;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _mqttService.SubscribeAsync(
            QueueConnection.WirenBoard("zigbee2mqtt/+", "test"),
            (message, ct) =>
            {
                _logger.LogInformation(
                    $"{message.Topic}: {message.Payload}");

                return Task.CompletedTask;
            },
            cancellationToken);
    }
}