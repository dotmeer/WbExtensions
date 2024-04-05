using System.Collections.Generic;

namespace WbExtensions.Domain.Alice.Capabilities.Mode;

public sealed class ModeCapabilityParameter : CapabilityParameter
{
    public ModeCapabilityParameter(string instance, IReadOnlyList<ModeCapabilityParameterMode> modes)
    {
        Instance = instance;
        Modes = modes;
    }

    public string Instance { get; }

    public IReadOnlyList<ModeCapabilityParameterMode> Modes { get; }
}