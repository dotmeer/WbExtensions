using System.Collections.Generic;

namespace WbExtensions.Domain.Home;

public sealed class DevicesSchema
{
    public IReadOnlyCollection<VirtualDevice> Devices { get; init; } = new List<VirtualDevice>(0);
}