namespace WbExtensions.Domain.Alice.Capabilities.Range;

public sealed class RangeCapabilityState : CapabilityState<double>
{
    public RangeCapabilityState(string instance) : base(instance)
    {
    }

    public bool? Relative { get; init; }
}