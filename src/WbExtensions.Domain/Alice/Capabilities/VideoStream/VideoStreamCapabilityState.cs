using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities.VideoStream;

public sealed class VideoStreamCapabilityState : CapabilityState<VideoStreamValue>
{
    public VideoStreamCapabilityState() : base(CapabilityStateInstances.GetStream)
    {
    }
}