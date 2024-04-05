using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities.OnOff;

public sealed class OnOffCapabilityState : CapabilityState<bool>
{
    public OnOffCapabilityState() : base(CapabilityStateInstances.On)
    {
    }
}