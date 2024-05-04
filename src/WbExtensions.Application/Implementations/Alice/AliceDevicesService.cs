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
        var virtualDevices = await _devicesRepository.GetAsync(
            actions.Select(_ => _.Id).ToList(),
            cancellationToken);

        var commands = new List<Command>(virtualDevices.Count);

       

        var result = new List<Device>(actions.Count);

        foreach (var action in actions)
        {
            var virtualDevice = virtualDevices.FirstOrDefault(_ => _.Id == action.Id);

            if (virtualDevice is null)
            {
                continue;
            }

            var device = new Device
            {
                Id = action.Id
            };

            foreach (var capability in action.Capabilities.Where(_ => _.State is not null))
            {
                var resultCapability = capability.GetUpdatedCapability();
                resultCapability.State!.ActionResult = CapabilityStateActionResult.Success();
                device.Capabilities.Add(resultCapability);
            }

            result.Add(device);
        }

        return result;
    }
}