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
                Room = GetRoom(virtualDevice.Room),
                Capabilities = virtualDevice.Controls.ToCapabilities().ToList(),
                Properties = virtualDevice.Controls.ToProperties().ToList()
            };
        }
    }

    private static string GetAliceDeviceType(VirtualDeviceType virtualDeviceType)
    {
        return virtualDeviceType switch
        {
            VirtualDeviceType.ClimateSensor => DeviceTypes.SensorClimate,
            VirtualDeviceType.DoorSensor => DeviceTypes.SensorOpen,
            VirtualDeviceType.Light => DeviceTypes.Light,
            VirtualDeviceType.OpenableCurtain => DeviceTypes.OpenableCurtain,
            _ => DeviceTypes.Other
        };
    }

    private static string? GetRoom(Room? room)
    {
        return room switch
        {
            Room.Balcony => "Балкон",
            Room.Bathroom => "Ванная",
            Room.Bedroom => "Спальня",
            Room.Hall => "Коридор",
            Room.Kitchen => "Кухня",
            Room.Livingroom => "Гостиная",
            Room.Toilet => "Туалет",
            _ => null
        };
    }

}