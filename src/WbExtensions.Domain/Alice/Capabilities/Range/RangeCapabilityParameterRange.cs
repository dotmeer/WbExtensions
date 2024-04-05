namespace WbExtensions.Domain.Alice.Capabilities.Range;

public sealed class RangeCapabilityParameterRange
{
    public RangeCapabilityParameterRange(double min, double max, double precision = 1)
    {
        Min = min;
        Max = max;
        Precision = precision;
    }

    public double Min { get; }

    public double Max { get; }

    public double Precision { get; }
}