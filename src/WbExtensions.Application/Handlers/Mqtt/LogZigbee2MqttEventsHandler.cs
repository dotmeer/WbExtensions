using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.Handlers.Mqtt;

public sealed class LogZigbee2MqttEventsHandler : IMqttHandler
{
    private readonly ILogger<LogZigbee2MqttEventsHandler> _logger;

    public LogZigbee2MqttEventsHandler(ILogger<LogZigbee2MqttEventsHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Topic}: {Payload}", message.Topic, message.Payload);

        return Task.CompletedTask;
    }
}