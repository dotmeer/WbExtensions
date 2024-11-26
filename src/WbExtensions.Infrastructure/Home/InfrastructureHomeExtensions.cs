using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WbExtensions.Application.Interfaces.Home;

namespace WbExtensions.Infrastructure.Home;

internal static class InfrastructureHomeExtensions
{
    public static IServiceCollection SetupHome(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSingleton<IVirtualDevicesRepository, VirtualDevicesRepository>();

        return services;
    }
}