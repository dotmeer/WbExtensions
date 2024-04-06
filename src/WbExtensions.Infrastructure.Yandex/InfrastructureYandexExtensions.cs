using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Infrastructure.Yandex.Abstractions;
using WbExtensions.Infrastructure.Yandex.Implementations;
using WbExtensions.Infrastructure.Yandex.Settings;

namespace WbExtensions.Infrastructure.Yandex;

public static class InfrastructureYandexExtensions
{
    public static IServiceCollection SetupYandex(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("Yandex:UserService").Get<UserServiceSettings>()
                       ?? throw new ArgumentNullException(nameof(UserServiceSettings));

        services
            .AddSingleton(settings)
            .AddHttpClient()
            .AddSingleton<IUserService, UserService>();

        return services;
    }
}