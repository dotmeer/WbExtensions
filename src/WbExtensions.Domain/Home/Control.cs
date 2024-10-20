using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Domain.Home;

public sealed class Control
{
    public string VirtualControlName { get; init; } = default!;

    public ControlType Type { get; init; }

    public bool Reportable { get; init; } = true;

    public string Value { get; private set; } = default!;

    public void UpdateValue(string value)
    {
        Value = value;
    }
}