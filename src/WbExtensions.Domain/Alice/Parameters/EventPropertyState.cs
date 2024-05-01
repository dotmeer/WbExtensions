namespace WbExtensions.Domain.Alice.Parameters;

public sealed class EventPropertyState : PropertyState
{
    public EventPropertyState(string instance, string value = default!) : base(instance)
    {
        Value = value;
    }

    public string Value { get; private set; }
}