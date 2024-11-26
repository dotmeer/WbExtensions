using WbExtensions.Domain.Alice.Capabilities.OnOff;
using WbExtensions.Domain.Alice.Capabilities.Range;
using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Domain.Alice.Capabilities;

public sealed class Capability
{
    public Capability(
        string type,
        bool? retrievable,
        bool? reportable,
        CapabilityParameter? parameters,
        CapabilityState? state)
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
    public bool? Retrievable { get; }

    /// <summary>
    /// Оповещение через сервис уведомлений.
    /// </summary>
    public bool? Reportable { get; }

    public CapabilityParameter? Parameters { get; }

    /// <summary>
    /// Параметры состояния умения.
    /// </summary>
    public CapabilityState? State { get; }

    public Capability GetUpdatedCapability()
    {
        return new Capability(
            Type,
            null,
            null,
            null,
            State?.GetCopy());
    }

    public static Capability CreateOnOffCapability(bool value, bool reportable, bool split = false)
    {
        return new Capability(
            CapabilityTypes.OnOff,
            true,
            reportable,
            new OnOffCapabilityParameter(split),
            new OnOffCapabilityState
            {
                Value = value
            });
    }

    public static Capability CreateOpenRangeCapability(double value, bool reportable, double precision = 10)
    {
        return new Capability(
            CapabilityTypes.Range,
            true,
            reportable,
            new RangeCapabilityParameter(
                CapabilityStateInstances.Open,
                FloatPropertyUnits.Percent,
                true,
                new RangeCapabilityParameterRange(0, 100, precision)),
            new RangeCapabilityState(CapabilityStateInstances.Open)
            {
                Value = value
            });
    }

    public static Capability CreateTemperatureRangeCapability(double value, bool reportable, double precision = 1)
    {
        return new Capability(
            CapabilityTypes.Range,
            true,
            reportable,
            new RangeCapabilityParameter(
                CapabilityStateInstances.Temperature,
                FloatPropertyUnits.TemperatureCelsius,
                true,
                new RangeCapabilityParameterRange(4, 35, precision)),
            new RangeCapabilityState(CapabilityStateInstances.Temperature)
            {
                Value = value
            });
    }
}