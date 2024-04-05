namespace WbExtensions.Domain.Alice.Parameters;

public abstract class PropertyParameter
{
    public string Instance { get; }

    protected PropertyParameter(string instance)
    {
        Instance = instance;
    }
}