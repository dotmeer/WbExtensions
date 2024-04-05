using System.Collections.Generic;

namespace WbExtensions.Domain.Alice.Requests;

public sealed class GetUserDevicesStateRequest
{
    public IList<GetUserDevicesStateRequestItem> Devices { get; init; } = new List<GetUserDevicesStateRequestItem>(0);
}