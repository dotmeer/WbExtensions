using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Responses;

public sealed class Payload
{
    [JsonPropertyName("user_id")]
    public string UserId { get; init; } = default!;

    public IList<Device> Devices { get; init; } = new List<Device>(0);
}