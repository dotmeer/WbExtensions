using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WbExtensions.Infrastructure.Json.Converters;

public sealed class OutOnlyConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return default;
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        object? newValue = null;
        if (value is not null)
        {
            newValue = Convert.ChangeType(value, value.GetType(), CultureInfo.InvariantCulture);
        }

        JsonSerializer.Serialize(writer, newValue, options);
    }
}