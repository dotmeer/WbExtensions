using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Responses;

public class AliceResponse
{
    [JsonPropertyName("request_id")]
    public string RequestId { get; init; } = default!;
}