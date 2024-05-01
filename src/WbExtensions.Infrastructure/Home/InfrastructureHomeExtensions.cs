using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Home;

namespace WbExtensions.Infrastructure.Home;

internal static class InfrastructureHomeExtensions
{
    public static IServiceCollection SetupHome(this IServiceCollection services)
    {
        services
            .AddSingleton<IDevicesRepository, DevicesRepository>();

        return services;
    }
}