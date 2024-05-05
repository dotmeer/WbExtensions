using System.Collections.Generic;
using System.Text.Json.Serialization;
using WbExtensions.Domain.Alice.Capabilities;

namespace WbExtensions.Domain.Alice.Requests;

public sealed class SetUSerDevicesStateRequestItem
{
    public string Id { get; init; } = default!;

    [JsonPropertyName("custom_data")]
    public IDictionary<string, VirtualDeviceCustomData>? CustomData { get; init; }

    public IList<Capability> Capabilities { get; init; } = new List<Capability>(0);
}