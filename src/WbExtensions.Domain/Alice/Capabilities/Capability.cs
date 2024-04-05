namespace WbExtensions.Domain.Alice.Capabilities;

// TODO: фабрика для умений
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
}