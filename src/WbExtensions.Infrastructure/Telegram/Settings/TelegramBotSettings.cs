namespace WbExtensions.Infrastructure.Telegram.Settings;

internal sealed record TelegramBotSettings(
    string Token,
    string[] Admins);