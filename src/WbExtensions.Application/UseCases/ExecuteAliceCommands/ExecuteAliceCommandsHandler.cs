using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;
using WbExtensions.Domain.Mqtt;

namespace WbExtensions.Application.UseCases.ExecuteAliceCommands;

public sealed class ExecuteAliceCommandsHandler
{
    private string MqttClientName => nameof(ExecuteAliceCommandsHandler);

    private readonly IVirtualDevicesRepository _virtualDevicesRepository;

    private readonly IMqttService _mqttService;

    public ExecuteAliceCommandsHandler(
        IVirtualDevicesRepository virtualDevicesRepository,
        IMqttService mqttService)
    {
        _virtualDevicesRepository = virtualDevicesRepository;
        _mqttService = mqttService;
    }

    public async Task<IList<Device>> HandleAsync(
        ExecuteAliceCommandsRequest request,
        CancellationToken cancellationToken)
    {
        var commands = new List<Command>(request.Actions.SelectMany(a => a.Capabilities).Count());
        var result = new List<Device>(request.Actions.Count);

        foreach (var action in request.Actions)
        {
            var device = new Device
            {
                Id = action.Id
            };

            foreach (var capability in action.Capabilities.Where(c => c.State is not null))
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

    private async Task PublishUpdatesAsync(IReadOnlyCollection<Command> commands, CancellationToken cancellationToken)
    {
        foreach (var command in commands)
        {
            if (_virtualDevicesRepository.TryGetControl(command.Device, command.Control, out var virtualDevice, out var control))
            {
                string? topic;
                string? message;

                switch (control!.Type)
                {
                    case ControlType.Switch:
                        topic = $"/devices/{command.Device}/controls/{command.Control}/on";
                        control.UpdateValue((bool)command.Value ? "1" : "0");
                        message = control.Value;
                        break;

                    case ControlType.Position:
                        topic = $"zigbee2mqtt/{command.Device}/set";
                        control.UpdateValue(command.Value.ToString()!);
                        message = $"{{\"position\": {control.Value}}}";
                        break;

                    case ControlType.CurtainState:
                        topic = $"zigbee2mqtt/{command.Device}/set";
                        control.UpdateValue((bool)command.Value ? "OPEN" : "CLOSE");
                        message = $"{{\"state\": \"{control.Value}\"}}";
                        break;

                    default:
                        topic = null;
                        message = null;
                        break;
                }

                if (topic is not null && message is not null)
                {
                    await _mqttService.PublishAsync(
                        new QueueConnection(topic, MqttClientName),
                        message,
                        cancellationToken);
                }
            }
        }
    }
}