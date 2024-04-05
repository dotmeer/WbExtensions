using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Capabilities.Range;

public sealed class RangeCapabilityParameter : CapabilityParameter
{
    public RangeCapabilityParameter(string instance, 
        string? unit = null, 
        bool randomAccess = true, 
        RangeCapabilityParameterRange? range = null)
    {
        Instance = instance;
        Unit = unit;
        RandomAccess = randomAccess;
        Range = range;
    }

    public string Instance { get; }

    public string? Unit { get; }

    [JsonPropertyName("random_access")]
    public bool RandomAccess { get; }

    public RangeCapabilityParameterRange? Range { get; }
}