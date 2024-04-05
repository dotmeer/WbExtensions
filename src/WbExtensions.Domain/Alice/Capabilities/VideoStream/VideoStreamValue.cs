using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Capabilities.VideoStream;

public sealed class VideoStreamValue
{
    [JsonPropertyName("stream_url")]
    public string StreamValue { get; }

    public string Protocol { get; }

    public VideoStreamValue(string streamValue, string protocol)
    {
        StreamValue = streamValue;
        Protocol = protocol;
    }
}