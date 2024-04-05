using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class HsvColorSettingCapabilityState : CapabilityState<Hsv>
{
    public HsvColorSettingCapabilityState() : base(CapabilityStateInstances.Hsv)
    {
    }
}