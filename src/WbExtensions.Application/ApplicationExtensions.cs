using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Handlers.Mqtt;

namespace WbExtensions.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection SetupApplication(this IServiceCollection services)
    {
        services
            .AddSingleton<LogZigbee2MqttEventsHandler>()
            .AddSingleton<ParseZigbee2MqttEventsHandler>()
            .AddSingleton<SubscribeDevicesToMqttHandler>();

        services
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly);
            });

        return services;
    }
}