using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Application.Interfaces.Metrics;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Domain.Alice.Push;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Home;
using WbExtensions.Application.Helpers;
using System;
using Microsoft.Extensions.Logging;
using WbExtensions.Domain;
using WbExtensions.Application.Helpers.Alice.Converters;

namespace WbExtensions.Application.UseCases.UpdateVirtualDeviceState;

public sealed class UpdateVirtualDeviceStateHandler
{
    private readonly IVirtualDevicesRepository _virtualDevicesRepository;
    private readonly IPushService _pushService;
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly IMetricsService _metricsService;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly ILogger<UpdateVirtualDeviceStateHandler> _logger;

    public UpdateVirtualDeviceStateHandler(
        IVirtualDevicesRepository virtualDevicesRepository,
        IPushService pushService,
        ITelemetryRepository telemetryRepository,
        IMetricsService metricsService,
        IUserInfoRepository userInfoRepository,
        ILogger<UpdateVirtualDeviceStateHandler> logger)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
        _pushService = pushService;
        _telemetryRepository = telemetryRepository;
        _metricsService = metricsService;
        _userInfoRepository = userInfoRepository;
        _logger = logger;
    }

    public async Task HandleAsync(
        UpdateVirtualDeviceStateRequest request,
        CancellationToken cancellationToken)
    {
        var (virtualDeviceName, virtualControlName) = TopicNameHelper.ParseDeviceControlTopic(request.Topic);

        if (_virtualDevicesRepository.TryGetControl(virtualDeviceName, virtualControlName, out var virtualDevice, out var control))
        {
            if (control!.Value != request.Value
                && request.Value is not null)
            {
                // сохраняем в оперативной памяти
                control.Value = request.Value!;

                // сохраняем в БД
                await SaveAsync(virtualDeviceName, virtualControlName, request.Value, cancellationToken);

                // пишем метрики для численных значений
                PushMetrics(virtualDeviceName, virtualControlName, request.Value);

                // отправляем состояние в Яндекс
                if (control.Reportable)
                {
                    await PushUpdateToYandexAsync(virtualDevice!, control, cancellationToken);
                }
            }
        }
    }

    private async Task SaveAsync(string virtualDeviceName, string virtualControlName, string value, CancellationToken cancellationToken)
    {
        try
        {
            await _telemetryRepository.UpsertAsync(
                new Telemetry(virtualDeviceName, virtualControlName, value, DateTime.UtcNow),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while saving to DB for device '{Device}' and control '{Control}'",
                virtualDeviceName,
                virtualControlName);
        }
    }

    private void PushMetrics(string virtualDeviceName, string virtualControlName, string value)
    {
        try
        {
            if (double.TryParse(value, out var doubleValue))
            {
                _metricsService.SetGauge(
                    "mqtt_topic_values",
                    doubleValue,
                    new Dictionary<string, string>
                    {
                        ["device_name"] = virtualDeviceName,
                        ["control_name"] = virtualControlName
                    },
                    "Values from mqtt topics");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while saving telemetry for device '{Device}' and control '{Control}'",
                virtualDeviceName,
                virtualControlName);
        }
    }

    private async Task PushUpdateToYandexAsync(VirtualDevice virtualDevice, Control control, CancellationToken cancellationToken)
    {
        try
        {
            var controlList = new List<Control>
            {
                control
            };

            var devices = new List<Device>
            {
                new Device
                {
                    Id = virtualDevice.Id,
                    Properties = controlList.ToProperties().ToList(),
                    Capabilities = controlList.ToCapabilities().ToList()
                }
            };

            foreach (var userInfo in await _userInfoRepository.GetAsync(cancellationToken))
            {
                var pushRequest = new PushRequest(
                    new PushRequestPayload
                    {
                        UserId = userInfo.Id,
                        Devices = devices
                    });

                await _pushService.PushAsync(pushRequest, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while pushing to yandex for device '{Device}' and control '{Control}'",
                virtualDevice.VirtualDeviceName,
                control.VirtualControlName);
        }
    }
}