using System.Collections.Generic;

namespace WbExtensions.Domain.Alice.Requests;

public sealed class SetUSerDevicesStateRequestPayload
{
    public IList<SetUSerDevicesStateRequestItem> Devices { get; init; } = new List<SetUSerDevicesStateRequestItem>(0);
}