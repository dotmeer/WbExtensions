using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Helpers;
using WbExtensions.Domain;
using WbExtensions.Domain.Mqtt;
using WbExtensions.Infrastructure.Database.Abstractions;

namespace WbExtensions.Application.MqttHandlers;

public sealed class SaveTelemetryHandler : IMqttHandler
{
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly ILogger<SaveTelemetryHandler> _logger;

    public SaveTelemetryHandler(
        ITelemetryRepository telemetryRepository, 
        ILogger<SaveTelemetryHandler> logger)
    {
        _telemetryRepository = telemetryRepository;
        _logger = logger;
    }

    public async Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            if (message.Payload is null)
            {
                return;
            }

            var (device, control) = TopicNameHelper.ParseDeviceControlTopic(message.Topic);

            await _telemetryRepository.AddAsync(
                new Telemetry(device, control, message.Payload, DateTime.UtcNow),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error while saving telemetry for topic '{Topic}' with payload '{Payload}'", 
                message.Topic,
                message.Payload);
        }
    }
}