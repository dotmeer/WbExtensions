namespace WbExtensions.Domain;

public sealed class TelegramUser
{
    public long UserId { get; init; }

    public string? UserName { get; init; }

    public bool IsAllowed { get; set; }

    public bool IsAdmin { get; set; }
}