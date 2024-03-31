using System;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotmeer.WbExtensions.Infrastructure.Mqtt;

public static class InfrastructureMqttExtensions
{
    public static IServiceCollection SetupMqtt(this IServiceCollection services, IConfiguration configuration)
    {
        var mqttSettings = configuration.GetSection("MqttSettings").Get<MqttSettings>()
                           ?? throw new ArgumentNullException(nameof(MqttSettings));

        services
            .AddSingleton(mqttSettings)
            .AddSingleton<IMqttService, MqttService>();

        return services;
    }
}