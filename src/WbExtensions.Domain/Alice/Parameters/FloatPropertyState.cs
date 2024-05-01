namespace WbExtensions.Domain.Alice.Parameters;

public sealed class FloatPropertyState : PropertyState
{
    public FloatPropertyState(string instance, double value = default) : base(instance)
    {
        Value = value;
    }

    public double Value { get; private set; }
}