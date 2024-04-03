using dotmeer.WbExtensions.Application.MqttHandlers;
using dotmeer.WbExtensions.Infrastructure.Mqtt.Abstractions;
using dotmeer.WbExtensions.Service.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;

namespace dotmeer.WbExtensions.Service;

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