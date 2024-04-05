namespace WbExtensions.Domain.Alice.Capabilities.Toggle;

public sealed class ToggleCapabilityParameter : CapabilityParameter
{
    public ToggleCapabilityParameter(string instance)
    {
        Instance = instance;
    }

    public string Instance { get; }
}