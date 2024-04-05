namespace WbExtensions.Domain.Alice.Capabilities.ColorSetting;

public sealed class SceneColorSettingCapabilityParameter
{
    public SceneColorSettingCapabilityParameter(string id)
    {
        Id = id;
    }

    public string Id { get; }
}