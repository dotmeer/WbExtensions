using System;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.Interfaces.Mqtt;

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