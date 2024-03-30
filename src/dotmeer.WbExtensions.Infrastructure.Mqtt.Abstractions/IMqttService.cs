using System;
using System.Threading;
using System.Threading.Tasks;

namespace dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;

public interface IMqttService
{
    Task PublishAsync(
        QueueConnection connection,
        string payload,
        CancellationToken cancellationToken);

    Task SubscribeAsync(
        QueueConnection connection,
        Func<QueueMessage, CancellationToken, Task> receiveHandler,
        CancellationToken cancellationToken);
}