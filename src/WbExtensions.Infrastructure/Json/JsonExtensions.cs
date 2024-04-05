using System.Text.Json;
using System.Text.Json.Serialization;
using WbExtensions.Domain.Alice.Capabilities;
using WbExtensions.Domain.Alice.Parameters;
using WbExtensions.Infrastructure.Json.Converters;

namespace WbExtensions.Infrastructure.Json;

public static class JsonExtensions
{
    public static JsonSerializerOptions Configure(this JsonSerializerOptions options)
    {
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.Converters.Add(new CapabilityStateConverter());
        options.Converters.Add(new OutOnlyConverter<CapabilityParameter>());
        options.Converters.Add(new OutOnlyConverter<PropertyParameter>());
        options.Converters.Add(new OutOnlyConverter<PropertyState>());

        return options;
    }
}