using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Infrastructure.Database;
using WbExtensions.Infrastructure.Home;
using WbExtensions.Infrastructure.Metrics;
using WbExtensions.Infrastructure.Mqtt;
using WbExtensions.Infrastructure.Telegram;
using WbExtensions.Infrastructure.Yandex;

namespace WbExtensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection SetupInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .SetupDatabase(configuration)
            .SetupMetrics()
            .SetupMqtt(configuration)
            .SetupYandex(configuration)
            .SetupHome(configuration)
            .SetupTelegram(configuration);

        return services;
    }
}