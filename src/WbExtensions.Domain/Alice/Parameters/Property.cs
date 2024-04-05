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
}