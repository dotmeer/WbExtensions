using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Application.UseCases.UpdateVirtualDeviceState;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.Handlers.Mqtt;

public sealed class SubscribeDevicesToMqttHandler : IMqttHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscribeDevicesToMqttHandler> _logger;

    public SubscribeDevicesToMqttHandler(
        IMediator mediator,
        ILogger<SubscribeDevicesToMqttHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(
                new UpdateVirtualDeviceStateRequest(message.Topic, message.Payload),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating virtual devices from mqtt");
        }
    }
}