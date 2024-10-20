using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Application.UseCases.UpdateVirtualDeviceState;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.Handlers;

public sealed class SubscribeDevicesToMqttHandler : IMqttHandler
{
    private readonly UpdateVirtualDeviceStateHandler _handler;
    private readonly ILogger<SubscribeDevicesToMqttHandler> _logger;

    public SubscribeDevicesToMqttHandler(
        UpdateVirtualDeviceStateHandler handler,
        ILogger<SubscribeDevicesToMqttHandler> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _handler.HandleAsync(
                new UpdateVirtualDeviceStateRequest(message.Topic, message.Payload),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating virtual devices from mqtt");
        }
    }
}