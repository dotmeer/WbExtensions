using System.Collections.Generic;
using System.Linq;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Constants;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.Implementations.Alice.Converters;

internal static class DeviceConverter
{
    public static IEnumerable<Device> ToDevices(this IReadOnlyCollection<VirtualDevice> virtualDevices)
    {
        foreach (var virtualDevice in virtualDevices)
        {
            yield return new Device
            {
                Id = virtualDevice.Id,
                Name = virtualDevice.Name,
                Description = virtualDevice.Description ?? string.Empty,
                Type = GetAliceDeviceType(virtualDevice.Type),
                Room = virtualDevice.Room,
                Capabilities = virtualDevice.GetCapabilities().ToList(),
                Properties = virtualDevice.Controls.ToProperties().ToList(),
                CustomData = GetCustomData(virtualDevice)
            };
        }
    }
    
    private static string GetAliceDeviceType(VirtualDeviceType virtualDeviceType)
    {
        return virtualDeviceType switch
        {
            VirtualDeviceType.ClimateSensor => DeviceTypes.SensorClimate,
            VirtualDeviceType.DoorSensor => DeviceTypes.SensorOpen,
            VirtualDeviceType.Fan => DeviceTypes.ThermostatAc,
            VirtualDeviceType.Light => DeviceTypes.Light,
            VirtualDeviceType.OpenableCurtain => DeviceTypes.OpenableCurtain,
            _ => DeviceTypes.Other
        };
    }

    private static IDictionary<string, VirtualDeviceCustomData>? GetCustomData(this VirtualDevice virtualDevice)
    {
        switch (virtualDevice.Type)
        {
            case VirtualDeviceType.Fan:
            case VirtualDeviceType.Light:
            case VirtualDeviceType.OpenableCurtain:
                var result =  new Dictionary<string, VirtualDeviceCustomData>(virtualDevice.Controls.Count);
                foreach (var control in virtualDevice.Controls)
                {
                    var capabilityType = control.GetCapabilityType();
                    if(capabilityType is not null)
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