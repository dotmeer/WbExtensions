namespace WbExtensions.Domain.Telegram;

public sealed class TelegramUser
{
    public long UserId { get; init; }

    public string? UserName { get; init; }

    public Role Role { get; init; }
}