using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WbExtensions.Application.Implementations.Alice.Converters;
using WbExtensions.Application.Interfaces.Alice;
using WbExtensions.Application.Interfaces.Home;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Requests;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Implementations.Alice;

internal sealed class AliceDevicesService : IAliceDevicesService
{
    private readonly IDevicesRepository _devicesRepository;

    public AliceDevicesService(IDevicesRepository devicesRepository)
    {
        _devicesRepository = devicesRepository;
    }

    public async Task<IList<Device>> GetAsync(
        CancellationToken cancellationToken)
    {
        var virtualDevices = await _devicesRepository.GetAsync(cancellationToken);

        return virtualDevices.ToDevices().ToList();
    }

    public async Task<IList<Device>> GetAsync(
        IReadOnlyCollection<string> ids,
        CancellationToken cancellationToken)
    {
        var virtualDevices = await _devicesRepository.GetAsync(ids, cancellationToken);

        return virtualDevices.ToDevices().ToList();
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

        await _devicesRepository.UpdateStateAsync(commands, cancellationToken);

        return result;
    }
}