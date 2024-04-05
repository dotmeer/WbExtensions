using System.Collections.Generic;

namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class ScenesColorSettingCapabilityParameter
{
    public ScenesColorSettingCapabilityParameter(IReadOnlyList<SceneColorSettingCapabilityParameter> scenes)
    {
        Scenes = scenes;
    }

    public IReadOnlyList<SceneColorSettingCapabilityParameter> Scenes { get; }
}