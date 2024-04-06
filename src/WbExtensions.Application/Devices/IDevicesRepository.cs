using System.Collections.Generic;
using WbExtensions.Domain.Alice;
using WbExtensions.Domain.Alice.Requests;

namespace WbExtensions.Application.Devices;

public interface IDevicesRepository
{
    IList<Device> GetDevices(string[]? ids = null);

    IList<Device> UpdateDeviceState(List<SetUSerDevicesStateRequestItem> actions);
}