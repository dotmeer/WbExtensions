using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Mqtt;
using WbExtensions.Infrastructure.Mqtt.Settings;

namespace WbExtensions.Infrastructure.Mqtt;

internal static class InfrastructureMqttExtensions
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