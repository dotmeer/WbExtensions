using System.Text.Json.Serialization;

namespace WbExtensions.Domain.Alice.Capabilities;

public abstract class CapabilityState
{
    public string Instance { get; init; } = default!;

    [JsonPropertyName("action_result")]
    public CapabilityStateActionResult? ActionResult { get; set; }

    public abstract void SetValue(object? value);
    public abstract object? GetValue();

    public CapabilityState GetCopy()
    {
        return (CapabilityState)MemberwiseClone();
    }
}