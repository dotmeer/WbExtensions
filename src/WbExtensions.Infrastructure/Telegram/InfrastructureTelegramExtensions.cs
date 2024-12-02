using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WbExtensions.Application.Interfaces.Telegram;
using WbExtensions.Infrastructure.Telegram.Implementations;
using WbExtensions.Infrastructure.Telegram.Settings;

namespace WbExtensions.Infrastructure.Telegram;

internal static class InfrastructureTelegramExtensions
{
    public static IServiceCollection SetupTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        var telegramBotSettings = configuration.GetSection("TelegramBot").Get<TelegramBotSettings>()
                                  ?? throw new ArgumentNullException(nameof(TelegramBotSettings));

        services
            .AddSingleton(telegramBotSettings)
            .AddSingleton<ITelegramService, TelegramService>();

        return services;
    }
}