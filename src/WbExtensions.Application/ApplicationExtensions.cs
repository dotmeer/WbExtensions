using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.MqttHandlers;

namespace WbExtensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection SetupApplication(this IServiceCollection services)
    {
        services
            .AddSingleton<LogZigbee2MqttEventsHandler>()
            .AddSingleton<MqttDevicesControlsMetricsHandler>()
            .AddSingleton<ParseZigbee2MqttEventsHandler>();

        return services;
    }
}