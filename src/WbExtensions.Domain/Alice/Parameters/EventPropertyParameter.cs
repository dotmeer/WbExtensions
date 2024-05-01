using System.Collections.Generic;
using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Parameters;

public sealed class EventPropertyParameter : PropertyParameter
{
    private EventPropertyParameter(string instance, params EventPropertyParameterValue[] events)
        : base(instance)
    {
        Events = events;
    }

    public IReadOnlyList<EventPropertyParameterValue> Events { get; }

    public static EventPropertyParameter EventVibration()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventVibration,
            new EventPropertyParameterValue(EventPropertyEvents.Tilt),
            new EventPropertyParameterValue(EventPropertyEvents.Fail),
            new EventPropertyParameterValue(EventPropertyEvents.Vibration));
    }

    public static EventPropertyParameter EventOpen()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventOpen,
            new EventPropertyParameterValue(EventPropertyEvents.Opened),
            new EventPropertyParameterValue(EventPropertyEvents.Closed));
    }

    public static EventPropertyParameter EventButton()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventButton,
            new EventPropertyParameterValue(EventPropertyEvents.Click),
            new EventPropertyParameterValue(EventPropertyEvents.DoubleClick),
            new EventPropertyParameterValue(EventPropertyEvents.LongPress));
    }

    public static EventPropertyParameter EventMotion()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventMotion,
            new EventPropertyParameterValue(EventPropertyEvents.Detected),
            new EventPropertyParameterValue(EventPropertyEvents.NotDetected));
    }

    public static EventPropertyParameter EventSmoke()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventSmoke,
            new EventPropertyParameterValue(EventPropertyEvents.Detected),
            new EventPropertyParameterValue(EventPropertyEvents.NotDetected),
            new EventPropertyParameterValue(EventPropertyEvents.High));
    }

    public static EventPropertyParameter EventGas()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventGas,
            new EventPropertyParameterValue(EventPropertyEvents.Detected),
            new EventPropertyParameterValue(EventPropertyEvents.NotDetected),
            new EventPropertyParameterValue(EventPropertyEvents.High));
    }

    public static EventPropertyParameter EventBatteryLevel()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventBatteryLevel,
            new EventPropertyParameterValue(EventPropertyEvents.Low),
            new EventPropertyParameterValue(EventPropertyEvents.Normal));
    }

    public static EventPropertyParameter EventFoodLevel()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventFoodLevel,
            new EventPropertyParameterValue(EventPropertyEvents.Empty),
            new EventPropertyParameterValue(EventPropertyEvents.Low),
            new EventPropertyParameterValue(EventPropertyEvents.Normal));
    }

    public static EventPropertyParameter EventWaterLevel()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventWaterLevel,
            new EventPropertyParameterValue(EventPropertyEvents.Empty),
            new EventPropertyParameterValue(EventPropertyEvents.Low),
            new EventPropertyParameterValue(EventPropertyEvents.Normal));
    }

    public static EventPropertyParameter EventWaterLeak()
    {
        return new EventPropertyParameter(
            PropertyInstances.EventWaterLeak,
            new EventPropertyParameterValue(EventPropertyEvents.Dry),
            new EventPropertyParameterValue(EventPropertyEvents.Leak));
    }
}