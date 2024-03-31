using dotmeer.WbExtensions.Application.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace dotmeer.WbExtensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection SetupApplication(this IServiceCollection services)
    {
        services
            .AddSingleton<LogZigbee2MqttEventsJob>()
            .AddSingleton<MqttDevicesControlsMetricsJob>()
            .AddSingleton<ParseZigbee2MqttEventsJob>()
            .AddSingleton<BridgeToYandexJob>();

        return services;
    }
}