using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Handlers;
using WbExtensions.Application.UseCases.ExecuteAliceCommands;
using WbExtensions.Application.UseCases.GetDevicesForAlice;
using WbExtensions.Application.UseCases.GetUserId;
using WbExtensions.Application.UseCases.RemoveUser;
using WbExtensions.Application.UseCases.UpdateVirtualDeviceState;

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
            .AddSingleton<GetDevicesForAliceHandler>()
            .AddSingleton<ExecuteAliceCommandsHandler>()
            .AddSingleton<UpdateVirtualDeviceStateHandler>()
            .AddSingleton<RemoveUserHandler>()
            .AddSingleton<GetUserIdHandler>();

        return services;
    }
}