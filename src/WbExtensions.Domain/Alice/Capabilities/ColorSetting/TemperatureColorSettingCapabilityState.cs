using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class TemperatureColorSettingCapabilityState : CapabilityState<int>
{
    public TemperatureColorSettingCapabilityState() : base(CapabilityStateInstances.TemperatureK)
    {
    }
}