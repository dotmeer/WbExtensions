using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace dotmeer.WbExtensions.Infrastructure.Mqtt;

public static class InfrastructureMqttExtensions
{
    public static IServiceCollection SetupMqtt(this IServiceCollection services)
    {
        services.AddSingleton<IMqttService, MqttService>();

        return services;
    }
}