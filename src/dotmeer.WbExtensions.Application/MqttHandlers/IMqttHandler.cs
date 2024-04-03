using System.Threading;
using System.Threading.Tasks;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;

namespace dotmeer.WbExtensions.Application.MqttHandlers;

public interface IMqttHandler
{
    public Task HandleAsync(QueueMessage message, CancellationToken cancellationToken);
}