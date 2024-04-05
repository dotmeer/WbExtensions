using System.Collections.Generic;

namespace WbExtensions.Domain.Alice.Capabilities.VideoStream;

public sealed class VideoStreamCapabilityParameter : CapabilityParameter
{
    public VideoStreamCapabilityParameter()
    {
        Protocols = new []{ "hls" };
    }

    public IReadOnlyList<string> Protocols { get; }
}