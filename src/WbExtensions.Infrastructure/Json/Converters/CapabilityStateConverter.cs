using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Constants;

namespace WbExtensions.Infrastructure.Json.Converters;

public sealed class CapabilityStateConverter : JsonConverter<CapabilityState?>
{
    public override CapabilityState? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var instance = GetPropertyValue(reader, "instance");
        if (instance is not null
            && CapabilityStateInstances.InstanceCapabilityStateTypeMapping.TryGetValue(instance, out var type))
        {
            return (CapabilityState?)JsonSerializer.Deserialize(ref reader, type, options);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, CapabilityState? value, JsonSerializerOptions options)
    {
        object? newValue = null;
        if (value is not null)
        {
            newValue = Convert.ChangeType(value, value.GetType(), CultureInfo.InvariantCulture);
        }

        JsonSerializer.Serialize(writer, newValue, options);
    }

    private string? GetPropertyValue(Utf8JsonReader reader, string propertyName)
    {
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == propertyName
                                                               && reader.Read() && reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
        }

        return null;
    }
}