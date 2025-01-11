namespace WbExtensions.Domain.Telegram;

public sealed class TelegramUser
{
    public long UserId { get; init; }

    public string? UserName { get; init; }

    public Role Role { get; init; }

    public bool IsKnown => Role > Role.Unknown;

    public bool IsAdmin => Role == Role.Admin;

    public bool IsKeeper => Role == Role.Keeper;

    public bool IsUnknown => Role == Role.Unknown;
}