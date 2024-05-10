using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Yandex;
using WbExtensions.Infrastructure.Yandex.Implementations;
using WbExtensions.Infrastructure.Yandex.Settings;

namespace WbExtensions.Infrastructure.Yandex;

internal static class InfrastructureYandexExtensions
{
    public static IServiceCollection SetupYandex(this IServiceCollection services, IConfiguration configuration)
    {
        var userServiceSettings = configuration.GetSection("Yandex:UserService").Get<UserServiceSettings>()
                       ?? throw new ArgumentNullException(nameof(UserServiceSettings));
        var pushServiceSettings = configuration.GetSection("Yandex:PushService").Get<PushServiceSettings>()
                                  ?? throw new ArgumentNullException(nameof(PushServiceSettings));

        services
            .AddSingleton(userServiceSettings)
            .AddSingleton(pushServiceSettings)
            .AddHttpClient()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IPushService, PushService>();

        return services;
    }
}