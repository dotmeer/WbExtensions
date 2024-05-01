using System.Collections.Generic;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Capabilities.OnOff;
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
                case ControlType.SimpleLamp:
                    yield return new Capability(
                        CapabilityTypes.OnOff,
                        true,
                        false,
                        new OnOffCapabilityParameter(false),
                        new OnOffCapabilityState
                        {
                            Value = control.Value == "1"
                        });
                    break;

                default:
                    // TODO: реализовать для остальных
                    break;
            }
        }
    }
}