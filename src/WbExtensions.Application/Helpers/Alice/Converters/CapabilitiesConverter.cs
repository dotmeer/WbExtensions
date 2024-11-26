using System.Collections.Generic;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Constants;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.Helpers.Alice.Converters;

internal static class CapabilitiesConverter
{
    public static IEnumerable<Capability> ToCapabilities(this IReadOnlyCollection<Control> controls)
    {

        foreach (var control in controls)
        {
            switch (control.Type)
            {
                case ControlType.Switch:
                    yield return Capability.CreateOnOffCapability(control.IsEnabled(), control.Reportable);
                    break;

                case ControlType.Position:
                    yield return Capability.CreateOpenRangeCapability(control.ToDouble(), control.Reportable);
                    yield return Capability.CreateOnOffCapability(control.IsOpen(), control.Reportable, true);
                    break;

                case ControlType.Thermostat:
                    yield return Capability.CreateTemperatureRangeCapability(control.ToDouble(), true);
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
            ControlType.Thermostat => CapabilityTypes.Range,
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