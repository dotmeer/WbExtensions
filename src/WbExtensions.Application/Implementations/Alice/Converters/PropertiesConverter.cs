using System;
using System.Collections.Generic;
using System.Globalization;
using WbExtensions.Domain.Alice.Constants;
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
                    yield return new Property(
                        PropertyTypes.Float,
                        true,
                        false,
                        FloatPropertyParameter.FloatCo2Level(),
                        new FloatPropertyState(PropertyInstances.FloatCo2Level, control.ToFloat()));
                    break;

                case ControlType.Contact:
                    yield return new Property(
                        PropertyTypes.Event,
                        true,
                        false,
                        EventPropertyParameter.EventOpen(),
                        new EventPropertyState(PropertyInstances.EventOpen, control.ToOpen()));
                    break;

                case ControlType.Humidity:
                    yield return new Property(
                        PropertyTypes.Float,
                        true,
                        false,
                        FloatPropertyParameter.FloatHumidity(),
                        new FloatPropertyState(PropertyInstances.FloatHumidity, control.ToFloat()));
                    break;

                case ControlType.Illuminance:
                    yield return new Property(
                        PropertyTypes.Float,
                        true,
                        false,
                        FloatPropertyParameter.FloatIllumination(),
                        new FloatPropertyState(PropertyInstances.FloatIllumination, control.ToFloat()));
                    break;

                case ControlType.Occupancy:
                    yield return new Property(
                        PropertyTypes.Event,
                        true,
                        false,
                        EventPropertyParameter.EventMotion(),
                        new EventPropertyState(PropertyInstances.EventMotion, control.ToMotion()));
                    break;

                case ControlType.Temperature:
                    yield return new Property(
                        PropertyTypes.Float,
                        true,
                        false,
                        FloatPropertyParameter.FloatTemperatureCelsius(),
                        new FloatPropertyState(PropertyInstances.FloatTemperature, control.ToFloat()));
                    break;

                case ControlType.Voc:
                    yield return new Property(
                        PropertyTypes.Float,
                        true,
                        false,
                        FloatPropertyParameter.FloatTvoc(),
                        new FloatPropertyState(PropertyInstances.FloatTvoc, control.ToFloat()));
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