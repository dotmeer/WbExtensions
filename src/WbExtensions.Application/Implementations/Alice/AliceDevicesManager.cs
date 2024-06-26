﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Helpers;
using WbExtensions.Application.Implementations.Alice.Converters;
using WbExtensions.Application.Interfaces.Alice;
using WbExtensions.Application.Interfaces.Database;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Push;
using WbExtensions.Domain.Alice.Requests;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.Implementations.Alice;

internal sealed class AliceDevicesManager : IAliceDevicesManager
{
    private const string MqttClientName = "alice_devices_manager";

    private readonly IMqttService _mqttService;
    private readonly IDevicesRepository _devicesRepository;
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IPushService _pushService;

    private bool _inited;

    public AliceDevicesManager(
        IMqttService mqttService, 
        IDevicesRepository devicesRepository,
        ITelemetryRepository telemetryRepository,
        IUserInfoRepository userInfoRepository,
        IPushService pushService)
    {
        _mqttService = mqttService;
        _devicesRepository = devicesRepository;
        _telemetryRepository = telemetryRepository;
        _userInfoRepository = userInfoRepository;
        _pushService = pushService;

        _inited = false;
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        if (!_inited)
        {
            var telemetryValues = await _telemetryRepository.GetAsync(cancellationToken);

            foreach (var device in _devicesRepository.VirtualDevices)
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
            
            _inited = true;
        }
    }

    public Task<IList<Device>> GetAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IList<Device>>(
            _devicesRepository.VirtualDevices
                .ToDevices()
                .ToList());
    }

    public Task<IList<Device>> GetAsync(
        IReadOnlyCollection<string> ids,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IList<Device>>(
            _devicesRepository.VirtualDevices
                .Where(_ => ids.Contains(_.Id))
                .ToDevices()
                .ToList());
    }

    public async Task<IList<Device>> UpdateDevicesStateAsync(
        IReadOnlyCollection<SetUSerDevicesStateRequestItem> actions,
        CancellationToken cancellationToken)
    {
        var commands = new List<Command>(actions.SelectMany(_ => _.Capabilities).Count());
        var result = new List<Device>(actions.Count);

        foreach (var action in actions)
        {
            var device = new Device
            {
                Id = action.Id
            };
            
            foreach (var capability in action.Capabilities.Where(_ => _.State is not null))
            {
                var newControlValue = capability.State?.GetValue();
                var resultCapability = capability.GetUpdatedCapability();

                if (action.CustomData is not null
                    && action.CustomData.TryGetValue(capability.Type, out var virtualDeviceData)
                    && newControlValue is not null)
                {
                    commands.Add(new Command(
                        virtualDeviceData.VirtualDeviceName,
                        virtualDeviceData.VirtualControlName,
                        newControlValue));
                    resultCapability.State!.ActionResult = CapabilityStateActionResult.Success();
                }
                else
                {
                    resultCapability.State!.ActionResult = CapabilityStateActionResult.ErrorInternalError();
                }
                
                device.Capabilities.Add(resultCapability);
            }

            result.Add(device);
        }

        await PublishUpdatesAsync(commands, cancellationToken);

        return result;
    }

    private async Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var (virtualDeviceName, virtualControlName) = TopicNameHelper.ParseDeviceControlTopic(message.Topic);

        if (TryGetControl(virtualDeviceName, virtualControlName, out var virtualDevice, out var control))
        {
            if(control!.Value != message.Payload)
            {
                control.Value = message.Payload!;

                if(control.Reportable)
                {
                    await PushUpdateToYandexAsync(virtualDevice!, control, cancellationToken);
                }
            }
        }
    }

    private async Task PublishUpdatesAsync(IReadOnlyCollection<Command> commands, CancellationToken cancellationToken)
    {
        foreach (var command in commands)
        {
            if (TryGetControl(command.Device, command.Control, out var virtualDevice, out var control))
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

    private bool TryGetControl(string virtualDeviceName, string virtualControlName, out VirtualDevice? virtualDevice, out Control? control)
    {
        virtualDevice = _devicesRepository.VirtualDevices
            .FirstOrDefault(_ => _.VirtualDeviceName == virtualDeviceName
                                 && _.Controls.Any(c => c.VirtualControlName == virtualControlName));
        
        control = virtualDevice?.Controls
            .FirstOrDefault(_ => _.VirtualControlName == virtualControlName);

        return control is not null;
    }

    private async Task PushUpdateToYandexAsync(VirtualDevice virtualDevice, Control control, CancellationToken cancellationToken)
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
}