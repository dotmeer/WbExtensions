using System.Collections.Generic;
using System.Linq;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Constants;
using WbExtensions.Domain.Home;

namespace WbExtensions.Application.Helpers.Alice.Converters;

internal static class DeviceConverter
{
    public static IEnumerable<Device> ToDevices(this IEnumerable<VirtualDevice> virtualDevices)
    {
        foreach (var virtualDevice in virtualDevices)
        {
            yield return new Device
            {
                Id = virtualDevice.Id,
                Name = virtualDevice.Name,
                Description = virtualDevice.Description ?? string.Empty,
                Type = virtualDevice.Type,
                Room = virtualDevice.Room,
                Capabilities = virtualDevice.Controls.ToCapabilities().ToList(),
                Properties = virtualDevice.Controls.ToProperties().ToList(),
                CustomData = virtualDevice.GetCustomData()
            };
        }
    }

    private static IDictionary<string, VirtualDeviceCustomData>? GetCustomData(this VirtualDevice virtualDevice)
    {
        switch (virtualDevice.Type)
        {
            case DeviceTypes.Light:
            case DeviceTypes.Thermostat:
            case DeviceTypes.ThermostatAc:
            case DeviceTypes.OpenableCurtain:
                var result = new Dictionary<string, VirtualDeviceCustomData>(virtualDevice.Controls.Count);
                foreach (var control in virtualDevice.Controls)
                {
                    var capabilityType = control.GetCapabilityType();
                    if (capabilityType is not null)
                    {
                        result.Add(
                            capabilityType,
                            new VirtualDeviceCustomData(
                                virtualDevice.VirtualDeviceName,
                                control.VirtualControlName));
                    }
                }
                return result;

            default:
                return null;
        }
    }
}