namespace WbExtensions.Domain.Alice.Capabilities;

public abstract class CapabilityState<TValue> : CapabilityState
{
    public TValue? Value { get; set; }

    protected CapabilityState(string instance)
    {
        Instance = instance;
    }

    public override void SetValue(object? value)
    {
        Value = (TValue?)value;
    }

    public override object? GetValue()
    {
        return Value;
    }
}