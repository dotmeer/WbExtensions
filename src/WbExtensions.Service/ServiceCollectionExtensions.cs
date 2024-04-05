using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.MqttHandlers;
using WbExtensions.Domain.Mqtt;
using WbExtensions.Infrastructure.Mqtt.Abstractions;
using WbExtensions.Service.BackgroundServices;

namespace WbExtensions.Service;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMqttHandler<THandler>(
        this IServiceCollection services,
        QueueConnection connection)
        where THandler : IMqttHandler
    {
        services.AddHostedService(serviceProvider =>
            new RunMqttHandlerBackgroundService<THandler>(
                serviceProvider.GetService<IMqttService>()!,
                serviceProvider.GetService<THandler>()!,
                connection));

        return services;
    }
}