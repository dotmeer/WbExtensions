using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class RgbColorSettingCapabilityState : CapabilityState<int>
{
    public RgbColorSettingCapabilityState() : base(CapabilityStateInstances.Rgb)
    {
    }
}