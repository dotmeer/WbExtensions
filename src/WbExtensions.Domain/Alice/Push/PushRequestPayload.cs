using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Push;

public sealed class PushRequestPayload
{
    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = default!;

    [JsonPropertyName("devices")]
    public IReadOnlyCollection<Device> Devices { get; init; } = new List<Device>(0);
}