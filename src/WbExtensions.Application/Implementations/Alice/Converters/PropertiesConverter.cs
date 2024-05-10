using System;
using System.Collections.Generic;
using System.Globalization;
using WbExtensions.Domain.Alice.Parameters;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.Implementations.Alice.Converters;

internal static class PropertiesConverter
{
    public static IEnumerable<Property> ToProperties(this IReadOnlyCollection<Control> controls)
    {
        foreach (var control in controls)
        {
            switch (control.Type)
            {
                case ControlType.Co2:
                    yield return Property.CreateCo2LevelProperty(control.ToFloat());
                    break;

                case ControlType.Contact:
                    yield return Property.CreateOpenEventProperty(control.ToOpen());
                    break;

                case ControlType.Humidity:
                    yield return Property.CreateHumidityProperty(control.ToFloat());
                    break;

                case ControlType.Illuminance:
                    yield return Property.CreateIlluminanceProperty(control.ToFloat());
                    break;

                case ControlType.Occupancy:
                    yield return Property.CreateMotionEventProperty(control.ToMotion());
                    break;

                case ControlType.Temperature:
                    yield return Property.CreateTemperatureProperty(control.ToFloat());
                    break;

                case ControlType.Voc:
                    yield return Property.CreateVocLevelProperty(control.ToFloat());
                    break;
            }
        }
    }
    
    private static double ToFloat(this Control control)
    {
        return double.TryParse(control.Value, NumberFormatInfo.InvariantInfo, out var value)
            ? value
            : 0;
    }

    private static string ToMotion(this Control control)
    {
        return string.Equals(control.Value, "true", StringComparison.OrdinalIgnoreCase)
            ? "detected"
            : "not_detected";
    }

    private static string ToOpen(this Control control)
    {
        return string.Equals(control.Value, "true", StringComparison.OrdinalIgnoreCase)
            ? "closed"
            : "opened";
    }
}