using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class SceneColorSettingCapabilityState : CapabilityState<string>
{
    public SceneColorSettingCapabilityState() : base(CapabilityStateInstances.Scene)
    {
    }
}