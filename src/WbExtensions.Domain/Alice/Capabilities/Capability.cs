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

    public static Capability CreateOnOffCapability(bool value)
    {
        return new Capability(
            CapabilityTypes.OnOff,
            true,
            true,
            new OnOffCapabilityParameter(false),
            new OnOffCapabilityState
            {
                Value = value
            });
    }

    public static Capability CreateRangeCapability(double value, double precision = 10)
    {
        return new Capability(
            CapabilityTypes.Range,
            true,
            true,
            new RangeCapabilityParameter(
                "open",
                "unit.percent",
                true,
                new RangeCapabilityParameterRange(0, 100, precision)),
            new RangeCapabilityState("open")
            {
                Value = value
            });
    }
}