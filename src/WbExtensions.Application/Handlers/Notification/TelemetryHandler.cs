using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Models.Notification;
using WbExtensions.Domain;

namespace WbExtensions.Application.Handlers.Notification;

internal sealed class TelemetryHandler: INotificationHandler<ValueNotification>
{
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly ILogger<TelemetryHandler> _logger;

    public TelemetryHandler(
        ITelemetryRepository telemetryRepository,
        ILogger<TelemetryHandler> logger)
    {
        _telemetryRepository = telemetryRepository;
        _logger = logger;
    }

    public async Task Handle(ValueNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            if (notification.Value is not null)
            {
                await _telemetryRepository.UpsertAsync(
                    new Telemetry(
                        notification.VirtualDeviceName,
                        notification.VirtualControlName,
                        notification.Value,
                        DateTime.UtcNow),
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while saving to DB for device '{Device}' and control '{Control}'",
                notification.VirtualDeviceName,
                notification.VirtualControlName);
        }
    }
}