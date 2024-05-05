using System.Collections.Generic;
using System.Linq;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Capabilities.OnOff;
using WbExtensions.Domain.Alice.Capabilities.Range;
using WbExtensions.Domain.Alice.Constants;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.Implementations.Alice.Converters;

internal static class CapabilitiesConverter
{
    public static IEnumerable<Capability> GetCapabilities(this VirtualDevice virtualDevice)
    {
        switch (virtualDevice.Type)
        {
            case VirtualDeviceType.OpenableCurtain:
                var position = virtualDevice.Controls.FirstOrDefault(_ => _.Type == ControlType.Position);
                if (position is not null)
                {
                    yield return new Capability(
                        CapabilityTypes.Range,
                        true,
                        false,
                        new RangeCapabilityParameter(
                            "open",
                            "unit.percent",
                            true,
                            new RangeCapabilityParameterRange(0, 100, 10)),
                        new RangeCapabilityState("open")
                        {
                            Value = position.ToDouble()
                        });
                }

                var state = virtualDevice.Controls.FirstOrDefault(_ => _.Type == ControlType.CurtainState);
                if (state is not null)
                {
                    yield return new Capability(
                        CapabilityTypes.OnOff,
                        true,
                        false,
                        new OnOffCapabilityParameter(false),
                        new OnOffCapabilityState
                        {
                            Value = position.IsClosed()
                        });
                }
                break;

            default:
                foreach (var control in virtualDevice.Controls)
                {
                    switch (control.Type)
                    {
                        case ControlType.Switch:
                            yield return new Capability(
                                CapabilityTypes.OnOff,
                                true,
                                false,
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
                                false,
                                new RangeCapabilityParameter(
                                    "open",
                                    "unit.percent",
                                    true,
                                    new RangeCapabilityParameterRange(0, 100, 10)),
                                new RangeCapabilityState("open")
                                {
                                    Value = control.ToDouble()
                                });
                            break;

                        case ControlType.CurtainState:
                            yield return new Capability(
                                CapabilityTypes.OnOff,
                                true,
                                false,
                                new OnOffCapabilityParameter(false),
                                null);
                            break;
                    }
                }
                break;
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

    private static bool IsClosed(this Control? control)
    {
        return control is not null
            && control.ToDouble() == 0;
    }
}