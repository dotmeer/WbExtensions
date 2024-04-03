using dotmeer.WbExtensions.Application.MqttHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace dotmeer.WbExtensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection SetupApplication(this IServiceCollection services)
    {
        services
            .AddSingleton<BridgeToYandexHandler>()
            .AddSingleton<LogZigbee2MqttEventsHandler>()
            .AddSingleton<MqttDevicesControlsMetricsHandler>()
            .AddSingleton<ParseZigbee2MqttEventsHandler>();

        return services;
    }
}