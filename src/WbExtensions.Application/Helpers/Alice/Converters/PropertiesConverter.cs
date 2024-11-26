using System;
using System.Collections.Generic;
using System.Globalization;
using WbExtensions.Domain.Alice.Parameters;
using WbExtensions.Domain.Home;
using WbExtensions.Domain.Home.Enums;

namespace WbExtensions.Application.Helpers.Alice.Converters;

internal static class PropertiesConverter
{
    public static IEnumerable<Property> ToProperties(this IReadOnlyCollection<Control> controls)
    {
        foreach (var control in controls)
        {
            switch (control.Type)
            {
                case ControlType.Co2:
                    yield return Property.CreateCo2LevelProperty(control.ToFloat(), control.Reportable);
                    break;

                case ControlType.Contact:
                    yield return Property.CreateOpenEventProperty(control.ToOpen(), control.Reportable);
                    break;

                case ControlType.Humidity:
                    yield return Property.CreateHumidityProperty(control.ToFloat(), control.Reportable);
                    break;

                case ControlType.Illuminance:
                    yield return Property.CreateIlluminanceProperty(control.ToFloat(), control.Reportable);
                    break;

                case ControlType.Occupancy:
                    yield return Property.CreateMotionEventProperty(control.ToMotion(), control.Reportable);
                    break;

                case ControlType.Temperature:
                    yield return Property.CreateTemperatureProperty(control.ToFloat(), control.Reportable);
                    break;

                case ControlType.Voc:
                    yield return Property.CreateVocLevelProperty(control.ToFloat(), control.Reportable);
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
               || string.Equals(control.Value, "1", StringComparison.OrdinalIgnoreCase)
            ? "detected"
            : "not_detected";
    }

    private static string ToOpen(this Control control)
    {
        return string.Equals(control.Value, "true", StringComparison.OrdinalIgnoreCase)
               || string.Equals(control.Value, "1", StringComparison.OrdinalIgnoreCase)
            ? "closed"
            : "opened";
    }
}