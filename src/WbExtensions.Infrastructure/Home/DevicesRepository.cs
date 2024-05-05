using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Helpers;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Infrastructure.Home;

internal sealed class DevicesRepository : IDevicesRepository
{
    private const string MqttClientName = "devices_repository";

    private readonly IMqttService _mqttService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ManualResetEvent _manualResetEvent;
    
    private ICollection<VirtualDevice> _devices;
    private bool _inited;

    public DevicesRepository(
        IMqttService mqttService, 
        IServiceProvider serviceProvider)
    {
        _mqttService = mqttService;
        _serviceProvider = serviceProvider;
        _manualResetEvent = new ManualResetEvent(false);

        _devices = Array.Empty<VirtualDevice>();
        _inited = false;
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if(!_inited)
        {
            _devices = _serviceProvider.GetService<IConfiguration>()
                           ?.GetSection("Schema:Devices").Get<ICollection<VirtualDevice>>()
                       ?? throw new ArgumentNullException(nameof(_devices));
            
            var telemetryRepository = _serviceProvider.GetService<ITelemetryRepository>()
                                      ?? throw new ArgumentNullException(nameof(ITelemetryRepository));
            var telemetryValues = await telemetryRepository.GetAsync(cancellationToken);

            foreach (var device in _devices)
            {
                foreach (var control in device.Controls)
                {
                    var telemetry = telemetryValues.FirstOrDefault(_ =>
                        _.Device == device.VirtualDeviceName
                        && _.Control == control.VirtualControlName);

                    if (telemetry is not null)
                    {
                        control.Value = telemetry.Value;
                    }
                }
            }

            await _mqttService.SubscribeAsync(
                new QueueConnection("/devices/+/controls/+", MqttClientName),
                HandleAsync,
                cancellationToken);

            // TODO: проверить, что блокировка работает корректно - юнит-тесты?
            _manualResetEvent.Set();
            _inited = true;
        }
    }
    
    public Task<IReadOnlyCollection<VirtualDevice>> GetAsync(IReadOnlyCollection<string> ids, CancellationToken cancellationToken)
    {
        WaitInitialization();

        var result = _devices
            .AsEnumerable();

        if (ids.Count > 0)
        {
            result = result
                .Where(_ => ids.Contains(_.Id));
        }

        return Task.FromResult((IReadOnlyCollection<VirtualDevice>)result.ToList());
    }

    public async Task UpdateStateAsync(IReadOnlyCollection<Command> commands, CancellationToken cancellationToken)
    {
        WaitInitialization();

        foreach (var command in commands)
        {
            if (TryGetControl(command.Device, command.Control, out var control))
            {
                QueueConnection? connection;
                string? controlValue;

                switch (control!.Type)
                {
                    case ControlType.Switch:
                        connection = new QueueConnection(
                            $"/devices/{command.Device}/controls/{command.Control}/on",
                            MqttClientName);
                        controlValue = (bool)command.Value ? "1" : "0";
                        break;

                    case ControlType.Position:
                        connection = new QueueConnection(
                            $"zigbee2mqtt/{command.Device}/set",
                            MqttClientName);
                        controlValue = $"{{\"position\": {command.Value}}}";
                        break;

                    case ControlType.CurtainState:
                        connection = new QueueConnection(
                            $"zigbee2mqtt/{command.Device}/set",
                            MqttClientName);
                        controlValue = $"{{\"state\": \"{((bool)command.Value ? "OPEN" : "CLOSE")}\"}}";
                        break;

                    default:
                        connection = null;
                        controlValue = null;
                        break;
                }

                if (connection is not null && controlValue is not null)
                {
                    control.Value = controlValue;
                    await _mqttService.PublishAsync(
                        connection,
                        controlValue,
                        cancellationToken);
                }
            }
        }
    }

    private Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var (virtualDeviceName, virtualControlName) = TopicNameHelper.ParseDeviceControlTopic(message.Topic);
        
        if (TryGetControl(virtualDeviceName, virtualControlName, out var control))
        {
            control!.Value = message.Payload!;
        }

        return Task.CompletedTask;
    }

    private void WaitInitialization()
    {
        if (!_manualResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
        {
            throw new SynchronizationLockException("Устройства все еще заблокированы");
        }
    }

    private bool TryGetControl(string virtualDeviceName, string virtualControlName, out Control? control)
    {
        control = _devices
            .Where(_ => _.VirtualDeviceName == virtualDeviceName)
            .SelectMany(_ => _.Controls)
            .FirstOrDefault(_ => _.VirtualControlName == virtualControlName);

        return control is not null;
    }
}