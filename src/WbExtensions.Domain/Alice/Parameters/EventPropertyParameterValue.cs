namespace WbExtensions.Domain.Alice.Parameters;

public sealed class EventPropertyParameterValue
{
    public EventPropertyParameterValue(string value)
    {
        Value = value;
    }

    public string Value { get; }
}