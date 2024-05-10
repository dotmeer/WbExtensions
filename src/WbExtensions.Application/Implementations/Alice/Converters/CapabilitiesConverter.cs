using System.Collections.Generic;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Capabilities.OnOff;
using WbExtensions.Domain.Alice.Capabilities.Range;
using WbExtensions.Domain.Alice.Constants;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.Implementations.Alice.Converters;

internal static class CapabilitiesConverter
{
    public static IEnumerable<Capability> ToCapabilities(this IReadOnlyCollection<Control> controls)
    {

        foreach (var control in controls)
        {
            switch (control.Type)
            {
                case ControlType.Switch:
                    yield return new Capability(
                        CapabilityTypes.OnOff,
                        true,
                        true,
                        new OnOffCapabilityParameter(false),
                        new OnOffCapabilityState
                        {
                            Value = control.IsEnabled()
                        });
                    break;

                case ControlType.Position:
                    yield return new Capability(
                        CapabilityTypes.Range,
                        true,
                        true,
                        new RangeCapabilityParameter(
                            "open",
                            "unit.percent",
                            true,
                            new RangeCapabilityParameterRange(0, 100, 10)),
                        new RangeCapabilityState("open")
                        {
                            Value = control.ToDouble()
                        });
                    yield return new Capability(
                        CapabilityTypes.OnOff,
                        true,
                        true,
                        new OnOffCapabilityParameter(false),
                        new OnOffCapabilityState
                        {
                            Value = control.IsOpen()
                        });
                    break;
            }
        }
    }
    
    public static string? GetCapabilityType(this Control control)
    {
        return control.Type switch
        {
            ControlType.Switch => CapabilityTypes.OnOff,
            ControlType.Position => CapabilityTypes.Range,
            ControlType.CurtainState => CapabilityTypes.OnOff,
            _ => null
        };
    }

    private static bool IsEnabled(this Control control)
    {
        return control.Value == "1";
    }

    private static double ToDouble(this Control control)
    {
        return double.TryParse(control.Value, out var value)
            ? value
            : 0;
    }

    private static bool IsOpen(this Control? control)
    {
        return control is not null
            && control.ToDouble() >= 50;
    }
}