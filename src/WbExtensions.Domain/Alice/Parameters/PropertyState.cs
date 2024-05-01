using System;

namespace WbExtensions.Domain.Alice.Parameters;

public abstract class PropertyState
{
    public string Instance { get; }

    protected PropertyState(string instance)
    {
        Instance = instance;
    }
}