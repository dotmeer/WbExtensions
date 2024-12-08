using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WbExtensions.Application.Interfaces.Home;

namespace WbExtensions.Infrastructure.Home;

internal static class InfrastructureHomeExtensions
{
    public static IServiceCollection SetupHome(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<VirtualDevicesRepository>()
            .AddSingleton<IVirtualDevicesRepository>(sp => sp.GetRequiredService<VirtualDevicesRepository>())
            .AddSingleton<IInitializer>(sp => sp.GetRequiredService<VirtualDevicesRepository>());

        return services;
    }
}