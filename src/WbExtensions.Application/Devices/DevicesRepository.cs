using System;
using System.Collections.Generic;
using System.Linq;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Capabilities.OnOff;
using WbExtensions.Domain.Alice.Constants;
using WbExtensions.Domain.Alice.Parameters;
using WbExtensions.Domain.Alice.Requests;

namespace WbExtensions.Application.Devices;

internal sealed class DevicesRepository : IDevicesRepository
{
    private readonly IList<Device> _devices;

    public DevicesRepository()
    {
        _devices = InitDevices().ToList();
    }

    public IList<Device> GetDevices(string[]? ids = null)
    {
        if (ids is null
            || ids.Length == 0)
        {
            return _devices;
        }

        var result = _devices
            .Where(_ => ids.Contains(_.Id))
            .ToList();

        foreach (var device in result)
        {
            var property = device.Properties.FirstOrDefault(_ => _.Type == PropertyTypes.Float);

            property?.State.UpdateValue(new Random().Next(0, 100));
        }

        return result;
    }

    public IList<Device> UpdateDeviceState(List<SetUSerDevicesStateRequestItem> actions)
    {
        var result = new List<Device>();

        foreach (var action in actions)
        {
            var device = _devices.FirstOrDefault(_ => _.Id == action.Id);

            if (device is null)
            {
                continue;
            }

            var updatedDevice = device.GetUpdatedDevice();

            foreach (var capability in action.Capabilities.Where(_ => _.State is not null))
            {
                var deviceCapability = device.Capabilities
                    .FirstOrDefault(_ => _.Type == capability.Type && _.State is not null);

                if (deviceCapability is null)
                {
                    var failedCapability = capability.GetUpdatedCapability();
                    failedCapability.State!.ActionResult = CapabilityStateActionResult.ErrorInvalidAction();
                    updatedDevice.Capabilities.Add(failedCapability);
                    continue;
                }

                deviceCapability.State!.SetValue(capability.State!.GetValue());
                var updatedCapability = deviceCapability.GetUpdatedCapability();
                updatedCapability.State!.ActionResult = CapabilityStateActionResult.Success();
                updatedDevice.Capabilities.Add(updatedCapability);
            }

            if (updatedDevice.Capabilities.Count > 0)
            {
                result.Add(updatedDevice);
            }
        }

        return result;
    }

    private IEnumerable<Device> InitDevices()
    {
        yield return new()
        {
            Id = "test-lamp",
            Name = "Тестовая лампочка",
            Description = "Тестовая виртуальная лампочка, которая, как я надеюсь, будет работать",
            Type = DeviceTypes.Light,
            CustomData = new Dictionary<string, object>(0),
            Capabilities = new List<Capability>
            {
                new Capability(
                    CapabilityTypes.OnOff,
                    true,
                    false,
                    new OnOffCapabilityParameter(false),
                    new OnOffCapabilityState
                    {
                        Value = false
                    })
            },
            Properties = new List<Property>
            {
                new Property(
                    PropertyTypes.Float,
                    true,
                    false,
                    FloatPropertyParameter.FloatBatteryLevel(),
                    new FloatPropertyState(PropertyInstances.FloatBatteryLevel))
            }
        };
    }
}