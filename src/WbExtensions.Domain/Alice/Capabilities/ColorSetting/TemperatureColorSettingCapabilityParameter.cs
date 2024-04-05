namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class TemperatureColorSettingCapabilityParameter
{
    public TemperatureColorSettingCapabilityParameter(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public int Min { get; }

    public int Max { get; }
}