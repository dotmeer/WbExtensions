using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Implementations.Alice;
using WbExtensions.Application.Implementations.MqttHandlers;
using WbExtensions.Application.Interfaces.Alice;

namespace WbExtensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection SetupApplication(this IServiceCollection services)
    {
        services
            .AddSingleton<LogZigbee2MqttEventsHandler>()
            .AddSingleton<MqttDevicesControlsMetricsHandler>()
            .AddSingleton<ParseZigbee2MqttEventsHandler>()
            .AddSingleton<SaveTelemetryHandler>()
            .AddSingleton<IAliceDevicesService, AliceDevicesService>();

        return services;
    }
}