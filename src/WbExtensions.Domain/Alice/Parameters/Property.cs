using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Parameters;

public sealed class Property
{
    public Property(
        string type, 
        bool retrievable, 
        bool reportable, 
        PropertyParameter parameters, 
        PropertyState state)
    {
        Type = type;
        Retrievable = retrievable;
        Reportable = reportable;
        Parameters = parameters;
        State = state;
    }

    /// <summary>
    /// Тип умения.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Доступен запрос состояния.
    /// </summary>
    public bool Retrievable { get; }

    /// <summary>
    /// Оповещение через сервис уведомлений.
    /// </summary>
    public bool Reportable { get; }

    public PropertyParameter  Parameters { get; }

    public PropertyState State { get; }

    public static Property CreateCo2LevelProperty(double value, bool reportable)
    {
        return new Property(
            PropertyTypes.Float,
            true,
            reportable,
            FloatPropertyParameter.FloatCo2Level(),
            new FloatPropertyState(PropertyInstances.FloatCo2Level, value));
    }

    public static Property CreateOpenEventProperty(string value, bool reportable)
    {
        return new Property(
            PropertyTypes.Event,
            true,
            reportable,
            EventPropertyParameter.EventOpen(),
            new EventPropertyState(PropertyInstances.EventOpen, value));
    }

    public static Property CreateHumidityProperty(double value, bool reportable)
    {
        return new Property(
            PropertyTypes.Float,
            true,
            reportable,
            FloatPropertyParameter.FloatHumidity(),
            new FloatPropertyState(PropertyInstances.FloatHumidity, value));
    }

    public static Property CreateIlluminanceProperty(double value, bool reportable)
    {
        return new Property(
            PropertyTypes.Float,
            true,
            reportable,
            FloatPropertyParameter.FloatIllumination(),
            new FloatPropertyState(PropertyInstances.FloatIllumination, value));
    }

    public static Property CreateMotionEventProperty(string value, bool reportable)
    {
        return new Property(
            PropertyTypes.Event,
            true,
            reportable,
            EventPropertyParameter.EventMotion(),
            new EventPropertyState(PropertyInstances.EventMotion, value));
    }

    public static Property CreateTemperatureProperty(double value, bool reportable)
    {
        return new Property(
            PropertyTypes.Float,
            true,
            reportable,
            FloatPropertyParameter.FloatTemperatureCelsius(),
            new FloatPropertyState(PropertyInstances.FloatTemperature, value));
    }

    public static Property CreateVocLevelProperty(double value, bool reportable)
    {
        return new Property(
            PropertyTypes.Float,
            true,
            reportable,
            FloatPropertyParameter.FloatTvoc(),
            new FloatPropertyState(PropertyInstances.FloatTvoc, value));
    }
}